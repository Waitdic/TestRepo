namespace iVectorOne.Suppliers.MTS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using Microsoft.Extensions.Logging;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Suppliers.MTS.Models.Common;
    using iVectorOne.Suppliers.MTS.Models.Prebook;
    using iVectorOne.Suppliers.MTS.Models.Book;
    using iVectorOne.Suppliers.MTS.Models.Cancel;

    public partial class MTS : IThirdParty, ISingleSource
    {
        #region Constructor

        private readonly IMTSSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MTS> _logger;
        private readonly ISerializer _serializer;

        public MTS(IMTSSettings settings, HttpClient httpClient, ILogger<MTS> logger, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.MTS;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails, false);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        private readonly List<string> overrideCountriesList = new List<string>();

        private List<string> GetOverrideCountries(PropertyDetails oPropertyDetails)
        {
            if (overrideCountriesList.Count == 0)
            {
                string overrideCountries = _settings.OverrideCountries(oPropertyDetails);

                if (!string.IsNullOrWhiteSpace(overrideCountries))
                {
                    // split the string and add each one to _overrideCountries
                    foreach (string country in overrideCountries.Split('|'))
                    {
                        overrideCountriesList.Add(country);
                    }
                }
            }
            return overrideCountriesList;
        }
        #endregion

        #region Prebook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var success = true;
            var webRequest = new Request();

            try
            {
                var request = new MTSPrebookRequest
                {
                    EchoToken = "12866195988106211282233751",
                    ResStatus = "Quote",
                    Version = "0.1",
                    SchemaLocation = "OTA_HotelResRQ.xsd",
                    POS = GeneratePosTag(propertyDetails),
                    HotelReservations = new []{ CreateHotelReservation(propertyDetails) }
                };

                // get the add response 
                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.MTS,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    LogFileName = "PreBook",
                    CreateLog = true,
                };
                webRequest.SetRequest(_serializer.Serialize(request));
                await webRequest.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<MTSPrebookResponse>(webRequest.ResponseString);

                // get the costs from the response
                var costs = response.HotelReservations.Select(x => x.ResGlobalInfo.Total.AmountAfterTax).ToArray();
                if (costs[0].ToSafeMoney() != propertyDetails.LocalCost.ToSafeMoney())
                {
                    // Only returns total cost so divide by number of rooms
                    var cost = costs[0].ToSafeMoney() / propertyDetails.Rooms.Count;

                    foreach (var room in propertyDetails.Rooms)
                    {
                        room.LocalCost = cost;
                        room.GrossCost = cost;
                    }
                }

                // cancellation charges
                var cancellations = GetCancellations(propertyDetails, response);

                foreach (var cancellation in cancellations)
                {
                    propertyDetails.Cancellations.Add(cancellation);
                }

                // Grab the Errata
                if (response.HotelReservations.Any(x => x.ResGlobalInfo.BasicPropertyInfo != null))
                {
                    foreach (var errataNode in response.HotelReservations.SelectMany(x => x.ResGlobalInfo.BasicPropertyInfo.VendorMessages))
                    {
                        propertyDetails.Errata.AddNew(errataNode.Title, errataNode.SubSection.Paragraph.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Pre-Book", webRequest);
            }

            return success;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var reference = "";
            var overrideCountries = GetOverrideCountries(propertyDetails);
            var webRequest = new Request();

            try
            {
                var hotelReservation = CreateHotelReservation(propertyDetails, true);

                if (!string.IsNullOrEmpty(propertyDetails.BookingReference) || propertyDetails.Rooms.Any(x => !string.IsNullOrEmpty(x.SpecialRequest)))
                {
                    hotelReservation.ResGlobalInfo = new ResGlobalInfo();

                    if (!string.IsNullOrEmpty(propertyDetails.BookingReference))
                    {
                        if (overrideCountries.Contains(propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[2]))
                        {
                            hotelReservation.ResGlobalInfo.HotelReservationIDs = new[]
                            {
                                new HotelReservationID
                                {
                                    ResIDSourceContext = "Client",
                                    ResIDSource = _settings.OverRideID(propertyDetails),
                                    ResIDValue = propertyDetails.BookingReference.Replace(" ", "")
                                }
                            };
                        }
                    }

                    if (propertyDetails.Rooms.Any(x => !string.IsNullOrEmpty(x.SpecialRequest)))
                    {
                        hotelReservation.ResGlobalInfo.Comments = propertyDetails.Rooms.Select(room => new Comment
                        {
                            Name = "Applicant Notice",
                            Text = room.SpecialRequest
                        }).ToArray();
                    }
                }

                var request = new MTSBookRequest
                {
                    EchoToken = "12866195988106211282233751",
                    ResStatus = "Commit",
                    Version = "0.1",
                    SchemaLocation = "OTA_HotelResRQ.xsd",
                    POS = GeneratePosTag(propertyDetails),
                    HotelReservations = new []{ hotelReservation }
                };

                // get the response 
                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.MTS,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    LogFileName = "Book",
                    CreateLog = true,
                };
                webRequest.SetRequest(_serializer.Serialize(request));
                await webRequest.Send(_httpClient, _logger);
                var response = _serializer.DeSerialize<MTSBookResponse>(_serializer.CleanXmlNamespaces(webRequest.ResponseString));

                // check for any errors and save the booking code
                // p63 of documentation
                if (response.Errors.Length != 0)
                {
                    reference = "failed";
                }
                else
                {
                    reference = response.HotelReservations
                        .SelectMany(x => x.ResGlobalInfo.HotelReservationIDs)
                        .First(r => r.ResIDSource == "OTS")
                        .ResIDValue;
                }
            }
            catch (Exception ex)
            {
                reference = "failed";
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return reference;
        }

        #endregion

        #region Cancellations

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var webRequest = new Request();

            string sourceReference;
            if (!string.IsNullOrEmpty(propertyDetails.SourceReference))
            {
                sourceReference = propertyDetails.SourceReference;
            }
            else
            {
                return thirdPartyCancellationResponse;
            }

            try
            {
                // build the cancellation request
                var request = new MTSCancelRequest
                {
                    Version = "0.1",
                    CancelType = "Commit",
                    POS = GeneratePosTag(propertyDetails),
                    UniqueID =
                    {
                        Type = "14",
                        ID = sourceReference,
                        ID_Context = "Internal"
                    }
                };

                // get the response
                webRequest = new Request()
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.MTS,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    LogFileName = "Cancel",
                    CreateLog = true,
                };
                webRequest.SetRequest(_serializer.Serialize(request));
                await webRequest.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<MTSCancelResponse>(webRequest.ResponseString);
                if (response.Errors.Length > 0 || response.Status != "Committed")
                {
                    throw new Exception("Cancellation request did not return success");
                } 
                else 
                {
                    // Get a reference
                    thirdPartyCancellationResponse.TPCancellationReference = response.UniqueID.ID;
                    thirdPartyCancellationResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                thirdPartyCancellationResponse.TPCancellationReference = "Failed";
                thirdPartyCancellationResponse.Success = false;

                propertyDetails.Warnings.AddNew("Cancel Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", webRequest);
            }

            return thirdPartyCancellationResponse;
        }

        public Cancellations GetCancellations(PropertyDetails propertyDetails, MTSPrebookResponse response)
        {
            var cancellations = new Cancellations();

            try
            {
                if (response.HotelReservations.FirstOrDefault()?.ResGlobalInfo.CancelPenalties.FirstOrDefault()?.AmountPercent != null)
                {
                    // get cancellations in list so can sort them by release period
                    foreach (var node in response.HotelReservations.SelectMany(x => x.ResGlobalInfo.CancelPenalties))
                    {
                        var cancellation = new Cancellation();

                        var totalAmount = response.HotelReservations.First().ResGlobalInfo.Total.AmountAfterTax;
                        var dailyAmount = totalAmount / propertyDetails.Duration;
                        var percentage = node.AmountPercent?.Percent;
                        var numberOfNights = node.AmountPercent.NmbrOfNights;

                        if (node.Deadline.OffsetDropTime == "BeforeArrival")
                        {
                            // Get amount
                            if (percentage != null)
                            {
                                cancellation.Amount = numberOfNights > 0
                                    ? (0.01d * percentage * (double)dailyAmount * numberOfNights).ToSafeMoney() 
                                    : (0.01d * percentage * (double)totalAmount).ToSafeMoney();
                            }
                            else if (node.AmountPercent.Amount != 0)
                            {
                                cancellation.Amount = (decimal)node.AmountPercent.Amount;
                            }

                            // get end date of cancelpenalty
                            cancellation.EndDate = propertyDetails.ArrivalDate;

                            // get start date of cancelpenalty
                            if (node.Deadline.OffsetTimeUnit == "Day")
                            {
                                var days = node.Deadline.OffsetUnitMultiplier;
                                cancellation.StartDate = cancellation.EndDate.AddDays(-days);

                                // If 'afterbooking' overlaps 'beforearrival', start beforearrival day after afterbooking finishes
                                if (node.Deadline.OffsetDropTime == "AfterBooking")
                                {
                                    var afterBookingEndDate = DateTime.Now.AddDays(node.Deadline.OffsetMultiplier);

                                    if (afterBookingEndDate >= cancellation.StartDate)
                                    {
                                        cancellation.StartDate = afterBookingEndDate.AddDays(1d);
                                    }
                                }
                            }
                        }

                        cancellations.Add(cancellation);
                    }

                    // need to make it so do not overlap; if they do, put end date as being start date of next
                    for (var i = 0; i <= cancellations.Count - 1; i++)
                    {
                        if (i == 0) continue;
                        if (cancellations[i - 1].EndDate >= cancellations[i].StartDate)
                        {
                            cancellations[i - 1].EndDate = cancellations[i].StartDate.AddDays(-1);
                        }
                    }
                }

                cancellations.Solidify(SolidifyType.Sum);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Get Cancellations Exception", ex.ToString());
            }

            return cancellations;
        }

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            var result = new ThirdPartyCancellationFeeResult();
            var cancelCostRequest = new XmlDocument();
            var cancelCostResponse = new XmlDocument();
            var webRequest = new Request();

            try
            {
                // build the request
                var sbGetFees = new StringBuilder();

                // Build cancellationcost XML
                sbGetFees.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sbGetFees.Append("<OTA_CancelRQ xmlns=\"http://www.opentravel.org/OTA/2003/05\" Version=\"0.1\" CancelType=\"Quote\">");
                sbGetFees.Append(GeneratePosTag(propertyDetails));

                if (propertyDetails.SourceSecondaryReference != "")
                {
                    sbGetFees.AppendFormat("<UniqueID Type=\"14\" ID=\"{0}\" ID_Context=\"Internal\"/>", propertyDetails.SourceSecondaryReference);
                }
                else
                {
                    sbGetFees.AppendFormat("<UniqueID Type=\"14\" ID=\"{0}\" ID_Context=\"Internal\"/>", propertyDetails.SourceReference);
                }

                sbGetFees.Append("</OTA_CancelRQ>");

                // Send cancellation request to MTS and log
                cancelCostRequest.LoadXml(sbGetFees.ToString());

                // get the add response 
                webRequest = new Request()
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.MTS,
                    ContentType = ContentTypes.Application_x_www_form_urlencoded,
                    LogFileName = "Cancellation Costs",
                    CreateLog = true,
                };
                webRequest.SetRequest(cancelCostRequest);
                await webRequest.Send(_httpClient, _logger);

                cancelCostResponse = webRequest.ResponseXML;
                cancelCostResponse.LoadXml(cancelCostResponse.InnerXml.Replace(" xmlns=\"http://www.opentravel.org/OTA/2003/05\"", ""));

                if (cancelCostResponse.SelectSingleNode("OTA_CancelRS/CancelInfoRS/CancelRules/CancelRule/@Amount") is not null)
                {
                    // get the cancellation cost
                    result.Amount = cancelCostResponse.SelectSingleNode("OTA_CancelRS/CancelInfoRS/CancelRules/CancelRule/@Amount").InnerText.ToSafeMoney();
                    result.CurrencyCode = cancelCostResponse.SelectSingleNode("OTA_CancelRS/CancelInfoRS/CancelRules/CancelRule/@CurrencyCode").InnerText.ToSafeString();
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                propertyDetails.Warnings.AddNew("Cancellation Cost Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Pre-Cancel", webRequest);
            }

            return result;
        }

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        #endregion

        #region Booking Status Update

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        #endregion

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        #region Request Components

        private HotelReservation CreateHotelReservation(PropertyDetails propertyDetails, bool isBook = false)
        {
            var count = 0;
            var roomCount = 1;
            var roomStays = new List<RoomStay>();
            foreach (var roomDetails in propertyDetails.Rooms)
            {
                var resGuestRPHs = new List<ResGuestRPH>();
                foreach (var passenger in roomDetails.Passengers)
                {
                    count++;
                    resGuestRPHs.Add(new ResGuestRPH { RPH = count });
                }

                string ratePlan = roomDetails.ThirdPartyReference.Split("|")[3];
                roomStays.Add(new RoomStay
                {
                    RoomTypes = new[]
                    {
                        new RoomType
                        {
                            Code = roomDetails.ThirdPartyReference.Split("|")[0]
                        }
                    },
                    RatePlans = !string.IsNullOrEmpty(ratePlan) 
                        ? new []{new RatePlan { RatePlanCode = ratePlan } } 
                        : Array.Empty<RatePlan>(),
                    TimeSpan =
                    {
                        End = propertyDetails.DepartureDate.ToString("yyyy-MM-dd"),
                        Start = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd")
                    },
                    BasicPropertyInfo =
                    {
                        HotelCode = propertyDetails.TPKey
                    },
                    ResGuestRPHs = resGuestRPHs.ToArray(),
                    ServiceRPHs = new[]
                    {
                        new ServiceRPH
                        {
                            RPH = roomCount
                        }
                    }
                });

                roomCount++;
            }

            var services = propertyDetails.Rooms.Select((roomDetails, roomCount) => new Service
            {
                ServiceInventoryCode = roomDetails.ThirdPartyReference.Split('|')[1],
                ServiceRPH = roomCount + 1,
            });

            // need to loop for each person
            var resGuests = new List<ResGuest>();
            var guestCounter = 0;
            foreach (var passenger in propertyDetails.Rooms.SelectMany(roomDetails => roomDetails.Passengers))
            {
                guestCounter++;
                var ageQualifyingCode = passenger.PassengerType switch
                {
                    PassengerType.Adult => 10,
                    PassengerType.Child => 8,
                    PassengerType.Infant => 7,
                    _ => default
                };

                var guestCounts = new List<GuestCount>();
                switch (ageQualifyingCode)
                {
                    case 8: guestCounts.Add(new GuestCount { Age = passenger.Age });
                        break;
                    case 7: guestCounts.Add(new GuestCount { Age = 1 }); 
                        break;
                }

                resGuests.Add(new ResGuest
                {
                    AgeQualifyingCode = ageQualifyingCode,
                    ResGuestRPH = guestCounter,
                    Profiles = isBook ? CreateProfiles(passenger) : null,
                    GuestCounts = guestCounts.ToArray()
                });
            }

            return new HotelReservation
            {
                RoomStays = roomStays.ToArray(),
                Services = services.ToArray(),
                ResGuests = resGuests.ToArray(),
            };
        }

        private POS GeneratePosTag(PropertyDetails propertyDetails, string country = "")
        {
            var overrideCountries = GetOverrideCountries(propertyDetails);
            string id;

            if (string.IsNullOrEmpty(country))
            {
                if (!string.IsNullOrEmpty(propertyDetails.Rooms[0].ThirdPartyReference))
                {
                    country = propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[2];
                }
                else if (!string.IsNullOrEmpty(propertyDetails.ResortCode))
                {
                    country = propertyDetails.ResortCode.Split('|')[0];
                }
            }

            if (overrideCountries.Contains(country) && !string.IsNullOrEmpty(country))
            {
                id = _settings.OverRideID(propertyDetails);
            }
            else
            {
                id = _settings.User(propertyDetails);
            }

            return new POS
            {
                Source = new[]
                {
                    new Source
                    {
                        RequestorID =
                        {
                            ID_Context = _settings.ID_Context(propertyDetails),
                            ID = id,
                            Type = _settings.Type(propertyDetails)
                        },
                        BookingChannel = new BookingChannel { Type = 2 }
                    },
                    new Source
                    {
                        RequestorID =
                        {
                            Type = _settings.AuthenticationType(propertyDetails),
                            ID = _settings.AuthenticationID(propertyDetails),
                            MessagePassword = _settings.Password(propertyDetails)
                        }
                    }
                }
            };
        }

        private Profiles CreateProfiles(Passenger passenger)
        {
            return new Profiles
            {
                ProfileInfo =
                {
                    Profile =
                    {
                        Customer =
                        {
                            PersonName =
                            {
                                NamePrefix = passenger.Title,
                                GivenName = passenger.FirstName,
                                Surname = passenger.LastName
                            }
                        }
                    }
                }
            };
        }

        #endregion
    }
}