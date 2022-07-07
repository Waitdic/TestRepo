namespace ThirdParty.CSSuppliers.AbreuV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.AbreuV2.Models;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class AbreuV2 : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IAbreuV2Settings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AbreuV2> _logger;

        public string Source => ThirdParties.ABREUV2;

        public bool SupportsBookingSearch => false;

        public bool SupportsRemarks => true;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        #endregion

        #region Constructors

        public AbreuV2(IAbreuV2Settings settings, ISerializer serializer, HttpClient httpClient, ILogger<AbreuV2> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails oPropertyDetails)
        {
            //'Set up usual stuff
            var oRequests = new List<Request>();
            bool bSuccess = true;

            var oRequest = new XmlDocument();
            var prebookRequest = new Request(); // todo - run second search

            foreach (var oRoom in oPropertyDetails.Rooms)
            {
                string sCancellationPenalties = HttpUtility.HtmlDecode(oRoom.ThirdPartyReference.Split('|')[1]);

                oRoom.ThirdPartyReference = oRoom.ThirdPartyReference.Split('|')[0];

                var oCancellationPenalties = _serializer.DeSerialize<CancelPenalties>(sCancellationPenalties);

                int penaltiesCount = oCancellationPenalties.Penalties.Count();
                var cancellations = oCancellationPenalties.Penalties
                    .Select((x, i) => new
                    {
                        current = x,
                        next = (i + 1 == penaltiesCount) ? null : oCancellationPenalties.Penalties[i + 1]
                    })
                    .Select(x =>
                        {
                            return new Cancellation
                            {
                                Amount = x.current.Charge.Amount.ToSafeDecimal(),
                                StartDate = oPropertyDetails.ArrivalDate.AddDays(-x.current.Deadline.Units.ToSafeInt()),
                                EndDate = x.next == null
                                        ? oPropertyDetails.ArrivalDate
                                        : oPropertyDetails.ArrivalDate.AddDays(-x.next.Deadline.Units.ToSafeInt() - 1)
                            };
                        });

                oPropertyDetails.Cancellations.AddRange(cancellations);
            }

            oPropertyDetails.Cancellations.Solidify(SolidifyType.Sum);

            try
            {
                var prebookMessage = new OTA_HotelResRQ
                {
                    Transaction = "PreBooking",
                    DetailLevel = "2",
                    HotelRes =
                    {
                        Rooms = oPropertyDetails.Rooms.Select(oRoomDetail => {
                            return new RoomRq
                            {
                                RoomRate =
                                {
                                    BookingCode = oRoomDetail.ThirdPartyReference
                                }
                            };
                        }).ToList()
                    }
                };

                prebookRequest = await SendRequestAsync(prebookMessage, oPropertyDetails, _settings.BookingURL(oPropertyDetails), "PreBook");

                var oResponseXml = _serializer.CleanXmlNamespaces(prebookRequest.ResponseXML);
                var prebookResponse = _serializer.DeSerialize<OTA_BookingInfoRS>(oResponseXml);

                foreach (var error in prebookResponse.Errors)
                {
                    bSuccess = false;
                    oPropertyDetails.Errata.AddNew("Warning", error.Status);
                }
            }
            catch (Exception ex)
            {
                bSuccess = false;
                oPropertyDetails.Warnings.AddNew("Prebook Exception", ex.Message);
            }
            finally
            {
                oPropertyDetails.AddLog("Prebook", prebookRequest);
            }

            return bSuccess;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails oPropertyDetails)
        {
            //'Set up usual stuff
            string sLanguageID = _settings.LanguageID(oPropertyDetails);
            string sTarget = _settings.Target(oPropertyDetails);
            string sDateStamp = Now();

            var sBookingReference = "";
            var bookRequest = new Request();

            try
            {
                bool isLeadSet = false;
                var titleList = new List<string> { "mr", "mrs" };

                var oBookMessage = new OTA_HotelResRQ
                {
                    Transaction = "Booking",
                    UniqueID =
                    {
                        IdType = "ClientReference",
                        ID = $"{oPropertyDetails.BookingReference.TrimEnd()}"
                    },
                    HotelRes =
                    {
                        Rooms = oPropertyDetails.Rooms.Select(oRoom =>
                        {
                            var guestList = oRoom.Passengers.Select(oPassenger =>
                            {
                                string ageCode = "";
                                bool isLeadGuest = false;
                                string age = "";
                                string namePrefix = "";

                                switch(oPassenger.PassengerType)
                                {
                                    case PassengerType.Adult:
                                        if(!isLeadSet)
                                        {
                                            isLeadGuest = true;
                                            isLeadSet = true;
                                        }
                                        ageCode = Constant.AgeCodeAdult;
                                        break;
                                    case PassengerType.Child:
                                        ageCode = Constant.AgeCodeChild;
                                        age = $"{oPassenger.Age}";
                                        break;
                                    default:
                                        ageCode = Constant.AgeCodeChild;
                                        age = "1";
                                        break;
                                }

                                var sTitle = oPassenger.Title.ToLower();

                                switch(sTitle)
                                {
                                    case "master":
                                        namePrefix = "Mstr.";
                                        break;
                                    case "miss":
                                        namePrefix = "Ms.";
                                        break;
                                    default:
                                        namePrefix = titleList.Contains(sTitle)
                                                   ? "Mr."
                                                   : $"{oPassenger.Title.ToProperCase()}.";
                                        break;
                                }

                                return new Guest
                                {
                                    AgeCode = ageCode,
                                    LeadGuest = isLeadGuest?"1":"",
                                    Age = age,
                                    PersonName =
                                    {
                                        NamePrefix = namePrefix,
                                        GivenName = oPassenger.FirstName,
                                        Surname = oPassenger.LastName
                                    }
                                };
                            }).ToList();


                            return new RoomRq
                            {
                                RoomRate =
                                {
                                    BookingCode = oRoom.ThirdPartyReference
                                },
                                Guests = guestList
                            };
                        }).ToList()
                    }
                };

                //'Send booking request
                bookRequest = await SendRequestAsync(oBookMessage, oPropertyDetails, _settings.BookingURL(oPropertyDetails), "Book");

                //'Get and clean response
                var oBookingResponseXML = _serializer.CleanXmlNamespaces(bookRequest.ResponseXML);

                var oBookResponse = _serializer.DeSerialize<OTA_BookingInfoRS>(oBookingResponseXML);
                //'Grab supplier booking Reference
                string? sReservationResIDValue = oBookResponse.ResGlobalInfo.ResIDs.FirstOrDefault(id => string.Equals(id.IdType, "Reservation"))?.ID;
                string? sLocatorResIDValue = oBookResponse.HotelResList.SelectMany(hotelRes => hotelRes.HotelResInfo.HotelResIDs).FirstOrDefault(id => string.Equals(id.IdType, "Locator"))?.ID;

                sBookingReference = !string.IsNullOrEmpty(sReservationResIDValue) && !string.IsNullOrEmpty(sLocatorResIDValue)
                                  ? $"{sReservationResIDValue}|{sLocatorResIDValue}"
                                  : Constant.Failed;
            }
            catch (Exception)
            {
                return Constant.Failed;
            }
            finally
            {
                oPropertyDetails.AddLog("Booking", bookRequest);
            }

            return sBookingReference;
        }

        #endregion

        #region Cancel Booking

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails oPropertyDetails)
        {
            var oPreCancellationWebRequest = new Request();
            var oCancellationWebRequest = new Request();
            var oReturn = new ThirdPartyCancellationResponse();

            try
            {
                var oPreCancellationRequest = new OTA_CancelRQ
                {
                    Transaction = "PreCancel",
                    UniqueID =
                    {
                        IdType = "Locator",
                        ID = oPropertyDetails.SourceReference.Split('|')[1]
                    }
                };

                var sbCancellationRequest = new OTA_CancelRQ
                {
                    Transaction = "Cancel",
                    UniqueID =
                    {
                        IdType = "Reservation",
                        ID = oPropertyDetails.SourceReference.Split('|')[0]
                    }
                };

                oPreCancellationWebRequest = await SendRequestAsync(oPreCancellationRequest, oPropertyDetails, _settings.CancellationURL(oPropertyDetails), "PreCancellation");
                oCancellationWebRequest = await SendRequestAsync(sbCancellationRequest, oPropertyDetails, _settings.CancellationURL(oPropertyDetails), "Cancellation");

                var oPreCancellationResponseXML = _serializer.CleanXmlNamespaces(oPreCancellationWebRequest.ResponseXML);
                var oPreCancellationResponse = _serializer.DeSerialize<OTA_CancelRS>(oPreCancellationResponseXML);

                var oCancellationResponseXML = _serializer.CleanXmlNamespaces(oCancellationWebRequest.ResponseXML);
                var oCancellationResponse = _serializer.DeSerialize<OTA_BookingInfoRS>(oCancellationResponseXML);

                //'Find and set cancellation fee
                string sCancellationFee = oPreCancellationResponse.CancelInfoRS.CancellationCosts.Amount;
                sCancellationFee = sCancellationFee.Replace(".", "").Replace(",", ".").Split(' ')[0];
                decimal nCancellationFee = sCancellationFee.ToSafeDecimal();

                oReturn.Amount = nCancellationFee;
                oReturn.Success = true;
                oReturn.CurrencyCode = oPreCancellationResponse.CancelInfoRS.CancellationCosts.Currency;

                oReturn.TPCancellationReference = oCancellationResponse.ResGlobalInfo.ResIDs.FirstOrDefault(id => string.Equals(id.IdType, "Reservation")).ID;

                return oReturn;
            }
            catch
            {
                //'If cancellation unsuccessful
                return oReturn;
            }
            finally
            {
                // 'save the xml for the front end
                oPropertyDetails.AddLog("PreCancellation", oPreCancellationWebRequest);
                oPropertyDetails.AddLog("Cancellation", oCancellationWebRequest);
            }
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return "";
        }

        #endregion

        #region Booking Status Update

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new();
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        #endregion

        #region Helpers

        public static string Now()
        {
            string sDateStamp = DateTime.Now.ToString(Constant.TimestampFormat);
#if DEBUG
            //'Time should be constant for unit testing
            sDateStamp = new DateTime(2022, 12, 22).ToString(Constant.TimestampFormat);
#endif
            return sDateStamp;
        }

        public static Security BuildCredentials(IAbreuV2Settings settings, IThirdPartyAttributeSearch attribute)
        {
            return new Security
            {
                Context = settings.DatabaseName(attribute),
                Username = settings.User(attribute),
                Password = settings.Password(attribute)
            };
        }

        public async Task<Request> SendRequestAsync<T>(
            T oContent,
            PropertyDetails oPropertyDetails,
            string Url,
            string LogFilename
            ) where T : SoapContent, new()
        {
            var oEnvelope = new Envelope<T>
            {
                Header =
                {
                    Security = BuildCredentials(_settings, oPropertyDetails)
                },
                Body =
                {
                    Content = oContent
                }
            };

            var xmlDoc = _serializer.Serialize(oEnvelope);

            var oRequest = new Request
            {
                EndPoint = Url,
                Method = RequestMethod.POST,
                Source = ThirdParties.ABREUV2,
                LogFileName = LogFilename,
                CreateLog = true,
                UseGZip = true
            };

            oRequest.SetRequest(xmlDoc);
            await oRequest.Send(_httpClient, _logger);

            return oRequest;
        }

        #endregion
    }
}