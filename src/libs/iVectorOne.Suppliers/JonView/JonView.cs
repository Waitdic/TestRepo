namespace iVectorOne.Suppliers.JonView
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Suppliers.JonView.Models;
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

        public Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string bookingReference = "";
            var webRequest = new Request();

            try
            {
                // build request
                BookRequest bookRequest = BuildBookingRequest(propertyDetails);

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
                    RefItem = DateTime.Now.ToString(Constant.DateFormat),
                    AttItem = "host",
                    ResItem = propertyDetails.BookingReference
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
                    ProdCode = oRoomDetails.ThirdPartyReference,
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

        #endregion

        #region End session
        public void EndSession(PropertyDetails oPropertyDetails)
        {

        }

        #endregion

    }
}