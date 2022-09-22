namespace iVectorOne.Suppliers.JonView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Suppliers.JonView.Models;
    using iVectorOne.Suppliers.JonView.Models.Prebook;
    using Intuitive.Helpers.Serialization;

    public class JonView : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IJonViewSettings _settings;

        private readonly ISerializer _serializer;

        private readonly HttpClient _httpClient;

        private readonly ILogger<JonView> _logger;

        public bool SupportsRemarks => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool SupportsBookingSearch => false;

        public string Source => ThirdParties.JONVIEW;

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        #endregion

        #region Constructor

        public JonView(IJonViewSettings settings, HttpClient httpClient, ISerializer serializer, ILogger<JonView> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            try
            {
                await GetCancellationPolicyAsync(propertyDetails);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                return false;
            }

            return true;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var bookingReference = "";
            var webRequest = new Request();

            try
            {
                // build request
                var bookRequest = BuildBookingRequest(propertyDetails);

                // send the request
                webRequest = await SendWebRequestAsync(propertyDetails, "Book", bookRequest);
                var bookResponse = ExtractEnvelopeContent<BookResponse>(webRequest, _serializer);

                // get booking reference
                if (string.Equals(bookResponse.ActionSeg.Status, "C")) 
                {
                    bookingReference = bookResponse.ActionSeg.ResNumber;
                }

                // return reference or failed
                if (string.IsNullOrEmpty(bookingReference) || bookingReference.ToLower() == "error")
                {
                    bookingReference = "failed";
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                bookingReference = "failed";
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return bookingReference;
        }

        #endregion

        #region Cancellation

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var webRequest = new Request();

            try
            {
                // build request
                var cancelRequest = new CancelRequest
                {
                    ActionSeg = "CR",
                    ResInfo =
                    {
                        ResItem = propertyDetails.SourceReference
                    }
                };
                // Send the request
                webRequest = await SendWebRequestAsync(propertyDetails, "Cancel", cancelRequest);

                var cancelResponse = ExtractEnvelopeContent<CancelResponse>(webRequest, _serializer);

                // get reference
                if (string.Equals(cancelResponse.ActionSeg.Status, "D")) 
                {
                    thirdPartyCancellationResponse.TPCancellationReference = cancelResponse.ActionSeg.ResNumber;
                    thirdPartyCancellationResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString());
                thirdPartyCancellationResponse.Success = false;
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", webRequest);
            }

            return thirdPartyCancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails PropertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails oBookingSearchDetails)
        {
            return new();
        }

        public string CreateReconciliationReference(string sInputReference)
        {
            return "";
        }

        #endregion

        #region Booking Status Update

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        #endregion

        #region Support

        private BookRequest BuildBookingRequest(PropertyDetails propertyDetails) 
        {
            var guests = propertyDetails.Rooms.SelectMany((room, roomIdx) => room.Passengers.Select(pass => new { Guest = pass, RoomIdx = roomIdx}))
                .Select((guest, guestIdx) => new { GuestIdx = guestIdx, guest.Guest, GuestRoomIdx = guest.RoomIdx });

            var titles = Constant.Titles.Split(',');

            var bookRequest = new BookRequest
            {
                ActionSeg = "AR",
                CommitLevel = "1",
                ResInfo =
                {
                    RefItem = propertyDetails.BookingReference,
                    AttItem = "host"
                },
                PaxSegment = guests.Select(pax =>
                {
                    var guest = pax.Guest;
                    string sAge = "";
                    if (guest.PassengerType == PassengerType.Child) 
                    {
                        sAge = $"{guest.Age}";
                    }
                    if (guest.PassengerType == PassengerType.Infant) 
                    {
                        sAge = "1";
                    }

                    return new PaxRecord
                    {
                        PaxNum = $"{pax.GuestIdx + 1}",
                        PaxSeq = "",
                        TitleCode = titles.Contains(guest.Title.ToUpper())
                                                    ? guest.Title.ToUpper()
                                                    : Constant.DefaultGuestTitle,
                        FirstName = guest.FirstName,
                        LastName = guest.LastName,
                        Age = sAge,
                        Language = "EN"
                    };
                }).ToList(),
                BookSegment = propertyDetails.Rooms.Select((oRoomDetails, roomIdx) => new BookRecord
                {
                    BookNum = $"{roomIdx + 1}",
                    BookSeq = "",
                    ProdCode = oRoomDetails.ThirdPartyReference.Split("_")[0],
                    StartDate = propertyDetails.ArrivalDate.ToString(Constant.DateFormat),
                    Duration = $"{propertyDetails.Duration}",
                    Note = oRoomDetails.SpecialRequest,
                    PaxArray = guests.Aggregate("", (all, x) => $"{all}{(x.GuestRoomIdx == roomIdx ? "Y" : "N")}")
                }).ToList()
            };

            return bookRequest;
        }

        internal async Task<Request> SendWebRequestAsync<T>(PropertyDetails propertyDetails, string action, T messageObject) 
            where T : IMessageRq
        {            
            var soapRequest = CreateSoapRequest(_serializer, propertyDetails, _settings, messageObject);

            // Send the request
            var webRequest = new Request
            {
                EndPoint = _settings.GenericURL(propertyDetails),
                SoapAction = Constant.RequestSoapAction,
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Text_xml,
                Source = ThirdParties.JONVIEW,
                LogFileName = action,
                CreateLog = true
            };
            webRequest.SetRequest(soapRequest);

            await webRequest.Send(_httpClient, _logger);

            return webRequest;
        }

        internal static T ExtractEnvelopeContent<T>(Request webRequest, ISerializer serializer) 
            where T : IMessageRs
        {
            var soapResponse = serializer.DeSerialize<RsEnvelope>(
                    serializer.CleanXmlNamespaces(webRequest.ResponseXML));

            var soapContent = soapResponse.Body.CallResponse.Return;
            T result = (T)serializer.DeSerialize(typeof(T), soapContent);
            return result;
        }

        internal static XmlDocument CreateSoapRequest<T>(ISerializer serializer, IThirdPartyAttributeSearch tpAttributeSearch, IJonViewSettings settings, T message)
        {
            string soapContent = serializer.CleanXmlNamespaces(serializer.Serialize(message)).OuterXml;

            var envelope = new Envelope
            {
                Body =
                {
                    RequestCall =
                    {
                        AsType = "XML",
                        AsCache = "johnview_host",
                        ClientLocSeq = settings.ClientLoc(tpAttributeSearch),
                        User = settings.User(tpAttributeSearch),
                        Password = settings.Password(tpAttributeSearch),
                        EncodingStyle = "http://schemas.xmlsoap.org/soap/encoding/",
                        Message =
                        {
                            Content = soapContent
                        }
                    }
                }
            };

            var xmlEnvelope = serializer.Serialize(envelope);
            return xmlEnvelope;
        }

        public async Task GetCancellationPolicyAsync(PropertyDetails propertyDetails)
        {
            // create an array variable to hold the policy for each room
            var policies = new Cancellations[propertyDetails.Rooms.Count];

            // we'll need this regular expression too (declared here for efficiency)
            var dailyRegEx =
                new System.Text.RegularExpressions.Regex(@"on (?<type>\S+) (?<numberofdays>[0-9]+) day(\(s\))?");

            // loop through the rooms
            var loop = 0;
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                // build request
                var cancellationMessage = BuildCancellationURL(propertyDetails, roomDetails);

                // Send the request
                var webRequest = await SendWebRequestAsync(propertyDetails, "CancellationPolicy", cancellationMessage);

                // get response
                var cancellationResponse = ExtractEnvelopeContent<CancellationPolicyResponse>(webRequest, _serializer);

                // make sure we initialize the final policy for this room
                policies[loop] = new Cancellations();

                // check the status
                if (cancellationResponse.ActionSeg.Status == "C")
                {
                    // we need to get the end date and amount for each cancellation item, and add them to the final cancellation policy for this room
                    foreach (var cancellationNode in cancellationResponse.ProductListSeg.ListRecord.Select(x =>
                                 x.Cancellation.CanItem))
                    {
                        var note = cancellationNode.CanNote != string.Empty ? cancellationNode.CanNote : string.Empty;

                        // get start date
                        var startDate = cancellationNode.ToDays < 0
                            ? propertyDetails.ArrivalDate
                            : propertyDetails.ArrivalDate.AddDays(-cancellationNode.FromDays);

                        // get end date
                        var endDate = cancellationNode.ToDays < 0
                            ? propertyDetails.ArrivalDate
                            : propertyDetails.ArrivalDate.AddDays(-cancellationNode.ToDays);

                        // calculate the base amounts (the amounts we're going to use to get the final amount from)
                        var baseAmounts = new List<decimal>();
                        switch (cancellationNode.ChargeType ?? "")
                        {
                            case "EI":
                            case "ENTIRE ITEM":
                                {
                                    baseAmounts.Add(roomDetails.LocalCost);
                                    break;
                                }

                            case "P":
                            case "PER PERSON"
                                : // unfortunately we have to guess these as we are using the wrong search request (CT instead of CU)
                                {
                                    for (var i = 1; i <= roomDetails.Adults + roomDetails.Children; i++)
                                    {
                                        baseAmounts.Add(roomDetails.LocalCost / roomDetails.Adults + roomDetails.Children);
                                    }

                                    break;
                                }

                            case "DAILY":
                                {
                                    var dailyRegMatch = dailyRegEx.Match(note);
                                    var type = dailyRegMatch.Groups["type"].Value.ToLower();
                                    var numberOfDays = dailyRegMatch.Groups["numberofdays"].Value.ToSafeInt();

                                    // make sure we don't get zero rates (we have to be careful of this because if there is a 'stay X pay Y' special offer,
                                    // often the first night will be zero - and the cancellation fee should be based on the second night instead)
                                    var rates = Array.FindAll(roomDetails.ThirdPartyReference.Split('_')[1].Split('/'),
                                        (sRate) => sRate.ToSafeDecimal() != 0m);

                                    var ratesWeWant = new string[numberOfDays];

                                    var sourceIndex = type switch
                                    {
                                        // I'm not sure the type is ever anything other than 'first' but I thought I'd check here just in case
                                        "first" => 0,
                                        "last" => rates.Length - numberOfDays,
                                        _ => default
                                    };

                                    if (rates.Length > sourceIndex)
                                    {
                                        Array.ConstrainedCopy(rates, sourceIndex, ratesWeWant, 0, numberOfDays);
                                    }

                                    baseAmounts.AddRange(ratesWeWant.Select(rate => rate.ToSafeDecimal()));

                                    break;
                                }
                        }

                        // now, for each base amount, we're going to either add a value or a percentage to the final amount
                        var finalAmountForThisRule = 0d;
                        foreach (var amount in baseAmounts)
                        {
                            switch (cancellationNode.RateType ?? "")
                            {
                                case "D": // fixed amount in dollars
                                    {
                                        finalAmountForThisRule += (double)amount;
                                        break;
                                    }

                                case "P": // percentage of base amount
                                    {
                                        finalAmountForThisRule +=
                                            (double)amount * ((double)cancellationNode.CanRate / 100.0d);
                                        break;
                                    }
                            }
                        }

                        // we've got everything we need (finally) - now lets add it to the policy
                        policies[loop].AddNew(startDate, endDate, finalAmountForThisRule.ToSafeDecimal());
                    }

                    // solidify the policy(turns our random collection of rules into a proper(continuous) policy ready for merging)
                    policies[loop].Solidify(SolidifyType.Max, new DateTime(2099, 12, 31), roomDetails.LocalCost);
                }

                // increment the loop counter 
                loop++;
                propertyDetails.AddLog("Cancellation Costs", webRequest);
            }

            // merge the policies and add it to the booking
            propertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(policies);
        }

        private CancellationPolicyRequest BuildCancellationURL(PropertyDetails propertyDetails, RoomDetails roomDetails)
        {
            return new CancellationPolicyRequest
            {
                ActionSeg = "DP",
                SearchSeg =
                {
                    FromDate = propertyDetails.ArrivalDate.ToString("dd-MMM-yyyy"),
                    ToDate = propertyDetails.DepartureDate.ToString("dd-MMM-yyyy"),
                    ProdTypeCode = "All",
                    SearchType = "PROD",
                    ProductListSeg = { CodeItem = { ProductCode = roomDetails.ThirdPartyReference.Split('_')[0] } },
                    DisplayRestriction = "N",
                    DisplayPolicy = "Y",
                    DisplaySchDate = "N",
                    DisplayNameDetails = "Y",
                    DisplayUsage = "Y",
                    DisplayGeoCode = "Y",
                    DisplayDynamicRates = "Y",
                },
            };
        }

        #endregion

        #region End session
        public void EndSession(PropertyDetails oPropertyDetails)
        {

        }

        #endregion
    }
}