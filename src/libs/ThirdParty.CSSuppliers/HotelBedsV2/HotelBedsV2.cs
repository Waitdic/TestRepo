namespace ThirdParty.CSSuppliers.HotelBedsV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;

    public class HotelBedsV2 : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IHotelBedsV2Settings _settings;
        private readonly HttpClient _httpClient;
        private readonly ISecretKeeper _secretKeeper;
        private readonly ILogger<HotelBedsV2> _logger;

        public HotelBedsV2(IHotelBedsV2Settings settings, HttpClient httpClient, ISecretKeeper secretKeeper, ILogger<HotelBedsV2> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _secretKeeper = Ensure.IsNotNull(secretKeeper, nameof(secretKeeper));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.HOTELBEDSV2;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails oBookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails oPropertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public void EndSession(PropertyDetails oPropertyDetails)
        {

        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        bool IThirdParty.RequiresVCard(VirtualCardInfo info, string source) => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        private bool RequiresVCard;

        #endregion

        #region PreBook

        public bool PreBook(PropertyDetails propertyDetails)
        {
            bool prebookSuccess = true;
            var request = new Request();
            HotelBedsV2CheckRatesResponse response = new HotelBedsV2CheckRatesResponse();

            try
            {
                IThirdPartyAttributeSearch searchDetails = new SearchDetails
                {
                    ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
                };
                HotelBedsV2CheckRatesRequest checkRatesRequest = new HotelBedsV2CheckRatesRequest()
                {
                    language = _settings.ContentLanguage(searchDetails),
                };

                List<HotelBedsV2CheckRatesRequest.Room> rooms = new List<HotelBedsV2CheckRatesRequest.Room>();

                foreach (RoomDetails roomDetails in propertyDetails.Rooms)
                {
                    HotelBedsV2Reference reference = HotelBedsV2Reference.FromEncryptedString(roomDetails.ThirdPartyReference, _secretKeeper);
                    HotelBedsV2CheckRatesRequest.Room room = new HotelBedsV2CheckRatesRequest.Room()
                    {
                        rateKey = reference.RateKey,
                    };
                    rooms.Add(room);
                }
                checkRatesRequest.rooms = rooms.ToArray();

                request.EndPoint = _settings.CheckRatesURL(searchDetails);
                request.Method = eRequestMethod.POST;
                request.Source = ThirdParties.HOTELBEDSV2;
                request.ContentType = ContentTypes.Application_json;
                request.UseGZip = _settings.UseGZIP(searchDetails);
                request.CreateLog = propertyDetails.CreateLogs;
                request.LogFileName = "Prebook";
                request.Accept = "application/json";

                string requestString = JsonConvert.SerializeObject(checkRatesRequest);
                requestString = requestString.Replace("\"upselling\":null,", "");
                request.SetRequest(requestString);

                request.Headers.AddNew("Api-key", _settings.User(searchDetails));
                request.Headers.AddNew("X-Signature", HotelBedsV2Search.GetSignature(_settings.User(searchDetails), _settings.Password(searchDetails)));

                request.Send(_httpClient, _logger).RunSynchronously();

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    response = JsonConvert.DeserializeObject<HotelBedsV2CheckRatesResponse>(request.ResponseString);
                }
                else
                {
                    throw new Exception("Third Party Error, Response was Invalid", new Exception("Third Party Error, Response was Invalid"));
                }

                var processedRooms = new List<int>();

                foreach (HotelBedsV2CheckRatesResponse.Room room in response.hotel.rooms)
                {
                    foreach (HotelBedsV2CheckRatesResponse.Rate rate in room.rates)
                    {
                        bool rateUsed = false;
                        foreach (RoomDetails roomDetails in propertyDetails.Rooms
                            .Where(o =>
                                o.RoomTypeCode == room.code &&
                                HotelBedsV2Reference.FromEncryptedString(o.ThirdPartyReference, _secretKeeper).MealBasisCode == rate.boardCode &&
                                o.Adults == rate.adults &&
                                o.Children == rate.children))
                        {
                            //add in some checks for the case where we try and prebook 2 rooms with 
                            //same roomtype mealbasis and occupancies, so we only use info from each rate 
                            //once and don't just update every room in the proeprtydetails with matching info 
                            //every time a rate matches
                            if (!rateUsed && !processedRooms.Contains(roomDetails.PropertyRoomBookingID))
                            {
                                roomDetails.ThirdPartyReference = HotelBedsV2Reference.Create(rate.rateKey, rate.paymentType, rate.boardCode, _secretKeeper);
                                roomDetails.LocalCost = SafeTypeExtensions.ToSafeDecimal(rate.net);
                                roomDetails.GrossCost = SafeTypeExtensions.ToSafeDecimal(rate.net);


                                if (!string.IsNullOrWhiteSpace(rate.rateComments))
                                {
                                    propertyDetails.Errata.AddNew("Important Information", rate.rateComments);
                                }

                                var orderedCancellations = rate.cancellationPolicies.OrderBy(o => o.from.Date).ToList();
                                for (var i = 0; i < orderedCancellations.Count; i++)
                                {
                                    var cancellationCharge = orderedCancellations[i];
                                    var endDate = (i + 1) < orderedCancellations.Count ?
                                        orderedCancellations[i + 1].from.Date.AddDays(-1) : propertyDetails.ArrivalDate;
                                    propertyDetails.Cancellations.AddNew(cancellationCharge.from.Date, endDate, SafeTypeExtensions.ToSafeDecimal(cancellationCharge.amount));
                                }
                                rateUsed = true;
                                processedRooms.Add(roomDetails.PropertyRoomBookingID);
                            }
                        }
                    }
                }

                propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);
                propertyDetails.Cancellations.Solidify(SolidifyType.Sum);

                prebookSuccess = response.hotel != null;
                SetRequiresVCard(propertyDetails);
            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", exception.ToString());
                prebookSuccess = false;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(request.RequestString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.HOTELBEDSV2, "HotelBedsV2 PreBook Request", request.RequestString);
                }

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.HOTELBEDSV2, "HotelBedsV2 PreBook Response", request.ResponseString);
                }

            }

            return prebookSuccess;
        }

        #endregion

        #region RequiresVCardHelper

        private bool SetRequiresVCard(PropertyDetails propertyDetails)
        {
            RequiresVCard = propertyDetails.Rooms.Any(o => HotelBedsV2Reference.FromEncryptedString(o.ThirdPartyReference, _secretKeeper).PaymentType == "AT_HOTEL");
            return RequiresVCard;
        }

        #endregion

        #region Book

        public string Book(PropertyDetails propertyDetails)
        {
            string reference = "";
            var webRequest = new Request();
            try
            {
                var totalRooms = propertyDetails.Rooms.Count;

                var request = new HotelBedsV2CreateBookingRequest()
                {
                    holder = new HotelBedsV2CreateBookingRequest.Holder()
                    {
                        name = propertyDetails.LeadGuestFirstName,
                        surname = propertyDetails.LeadGuestLastName,
                    },
                    rooms = new HotelBedsV2CreateBookingRequest.Room[totalRooms],
                    tolerance = 2,
                    remark = propertyDetails.BookingComments.Any() ? propertyDetails.BookingComments.First().Text : "",
                    clientReference = propertyDetails.BookingReference
                };

                for (int i = 0; i <= totalRooms - 1; i++)
                {
                    var room = propertyDetails.Rooms.ElementAt(i);
                    HotelBedsV2Reference hotelBedsV2Reference = HotelBedsV2Reference.FromEncryptedString(room.ThirdPartyReference, _secretKeeper);
                    var hotelBedsRoom = new HotelBedsV2CreateBookingRequest.Room()
                    {
                        rateKey = hotelBedsV2Reference.RateKey
                    };
                    var hotelBedsPassengersList = new List<HotelBedsV2CreateBookingRequest.Pax>();
                    foreach (var passenger in room.Passengers)
                    {
                        var hotelBedsPassenger = new HotelBedsV2CreateBookingRequest.Pax()
                        {
                            name = passenger.FirstName,
                            surname = passenger.LastName,
                            roomId = SafeTypeExtensions.ToSafeString(1)
                        };

                        if (passenger.PassengerType == PassengerType.Adult)
                        {
                            hotelBedsPassenger.type = "AD";
                        }
                        else if (passenger.PassengerType == PassengerType.Infant || passenger.PassengerType == PassengerType.Child)
                        {
                            hotelBedsPassenger.type = "CH";
                        }
                        hotelBedsPassengersList.Add(hotelBedsPassenger);
                    }
                    hotelBedsRoom.paxes = hotelBedsPassengersList.ToArray();
                    request.rooms[i] = hotelBedsRoom;
                }

                IThirdPartyAttributeSearch searchDetails = new SearchDetails
                {
                    ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
                };

                webRequest.Source = ThirdParties.HOTELBEDSV2;
                webRequest.Method = eRequestMethod.POST;
                webRequest.EndPoint = _settings.BookingURL(searchDetails);

                if (RequiresVCard)
                {
                    webRequest.EndPoint = _settings.SecureBookingURL(searchDetails);
                    var paymentData = new PaymentData()
                    {
                        contactData = new ContactData()
                        {
                            email = propertyDetails.LeadGuestEmail,
                            phoneNumber = propertyDetails.LeadGuestPhone
                        },
                        paymentCard = new PaymentCard()
                        {
                            cardHolderName = propertyDetails.GeneratedVirtualCard.CardHolderName,
                            cardNumber = propertyDetails.GeneratedVirtualCard.CardNumber,
                            cardType = propertyDetails.GeneratedVirtualCard.CardTypeID.ToString(),
                            cardCVV = propertyDetails.GeneratedVirtualCard.CVV,
                            expiryDate = (propertyDetails.GeneratedVirtualCard.ExpiryMonth.Length == 2 ? propertyDetails.GeneratedVirtualCard.ExpiryMonth : "0" + propertyDetails.GeneratedVirtualCard.ExpiryMonth) + (propertyDetails.GeneratedVirtualCard.ExpiryYear.Length == 2 ? propertyDetails.GeneratedVirtualCard.ExpiryYear : propertyDetails.GeneratedVirtualCard.ExpiryYear.Substring(2, 2))
                        }
                    };
                    request.paymentData = paymentData;
                }

                webRequest.ContentType = ContentTypes.Application_json;
                webRequest.UseGZip = _settings.UseGZIP(searchDetails);
                webRequest.CreateLog = propertyDetails.CreateLogs;
                webRequest.LogFileName = "Book";
                webRequest.Accept = "application/json";

                string requestString = JsonConvert.SerializeObject(request);
                requestString = requestString.Replace(",\"paymentData\":null", "");
                requestString = requestString.Replace(",\"clientReference\":null", "");
                requestString = requestString.Replace(",\"remark\":null", "");
                webRequest.SetRequest(requestString);

                webRequest.Headers.AddNew("Api-key", _settings.User(searchDetails));
                webRequest.Headers.AddNew("X-Signature", HotelBedsV2Search.GetSignature(_settings.User(searchDetails), _settings.Password(searchDetails)));

                webRequest.Send(_httpClient, _logger).RunSynchronously();

                HotelBedsV2CreateBookingResponse response = new HotelBedsV2CreateBookingResponse();
                response = JsonConvert.DeserializeObject<HotelBedsV2CreateBookingResponse>(webRequest.ResponseString);

                if (!String.IsNullOrEmpty(response.booking.reference))
                {
                    reference = response.booking.reference;
                }
                else
                {
                    reference = "failed";
                }

                if (reference != "failed")
                {
                    propertyDetails.SupplierInfo = $"Payable through {response.booking.hotel.supplier.name}, acting as agent for the service operating company, details of which can be provided upon request. VAT: {response.booking.hotel.supplier.vatNumber} Reference: {response.booking.reference}";
                }

            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Book Exception", exception.ToString());
                reference = "failed";
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(webRequest.RequestString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.HOTELBEDSV2, "HotelBedsV2 Book Request", webRequest.RequestString);
                }

                if (!string.IsNullOrWhiteSpace(webRequest.RequestString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.HOTELBEDSV2, "HotelBedsV2 Book Response", webRequest.ResponseString);
                }
            }
            return reference;
        }

        #endregion

        #region Cancel

        public ThirdPartyCancellationResponse CancelBooking(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var cancellationResponse = new ThirdPartyCancellationResponse();
            try
            {
                IThirdPartyAttributeSearch searchDetails = new SearchDetails
                {
                    ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
                };

                request.Method = eRequestMethod.DELETE;
                request.EndPoint = _settings.CancellationURL(searchDetails);
                request.Source = ThirdParties.HOTELBEDSV2;
                request.ContentType = ContentTypes.Application_json;
                request.UseGZip = _settings.UseGZIP(searchDetails);
                request.CreateLog = propertyDetails.CreateLogs;
                request.LogFileName = "Cancel";
                request.Accept = "application/json";

                request.EndPoint = request.EndPoint.Replace("{{bookingReference}}", propertyDetails.SourceReference);

                request.Headers.AddNew("Api-key", _settings.User(searchDetails));
                request.Headers.AddNew("X-Signature", HotelBedsV2Search.GetSignature(_settings.User(searchDetails), _settings.Password(searchDetails)));

                request.Send(_httpClient, _logger).RunSynchronously();

                HotelBedsV2CancellationResponse response = new HotelBedsV2CancellationResponse();
                response = JsonConvert.DeserializeObject<HotelBedsV2CancellationResponse>(request.ResponseString);

                cancellationResponse.Success = response.booking.status == "CANCELLED";
                cancellationResponse.CurrencyCode = response.booking.currency;
                cancellationResponse.Amount = response.booking.hotel.cancellationAmount.ToSafeDecimal();
                cancellationResponse.TPCancellationReference = response.booking.cancellationReference;

            }
            catch (Exception exception)
            {
                cancellationResponse.Success = false;

                cancellationResponse.TPCancellationReference = "failed";

                propertyDetails.Warnings.AddNew("Cancellation Exception", exception.ToString());
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(request.EndPoint))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.HOTELBEDSV2, "HotelBedsV2 Cancellation Request", request.EndPoint);
                }

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.HOTELBEDSV2, "HotelBedsV2 Cancellation Response", request.ResponseString);
                }
            }

            return cancellationResponse;
        }

        #endregion

        #region GetCancellationCosts

        public ThirdPartyCancellationFeeResult GetCancellationCost(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var cancellationCostResponse = new ThirdPartyCancellationFeeResult();
            try
            {
                IThirdPartyAttributeSearch searchDetails = new SearchDetails
                {
                    ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
                };

                request.Method = eRequestMethod.DELETE;
                request.EndPoint = _settings.CancellationURL(searchDetails);
                request.Source = ThirdParties.HOTELBEDSV2;
                request.ContentType = ContentTypes.Application_json;
                request.UseGZip = _settings.UseGZIP(searchDetails);
                request.CreateLog = propertyDetails.CreateLogs;
                request.LogFileName = "Cancel";
                request.Accept = "application/json";

                request.EndPoint = request.EndPoint.Replace("{{bookingReference}}", propertyDetails.SourceReference);
                request.EndPoint = request.EndPoint.Replace("cancellationFlag=CANCELLATION", "cancellationFlag=SIMULATION");

                request.Headers.AddNew("Api-key", _settings.User(searchDetails));
                request.Headers.AddNew("X-Signature", HotelBedsV2Search.GetSignature(_settings.User(searchDetails), _settings.Password(searchDetails)));

                request.Send(_httpClient, _logger).RunSynchronously();

                var response = new HotelBedsV2CancellationResponse();
                response = JsonConvert.DeserializeObject<HotelBedsV2CancellationResponse>(request.ResponseString);

                cancellationCostResponse.Success = response.booking.status == "CANCELLED";
                cancellationCostResponse.CurrencyCode = response.booking.currency;
                cancellationCostResponse.Amount = response.booking.hotel.cancellationAmount.ToSafeDecimal();
            }
            catch (Exception exception)
            {
                cancellationCostResponse.Success = false;

                propertyDetails.Warnings.AddNew("GetCancellationCost Exception", exception.ToString());
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(request.EndPoint))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.HOTELBEDSV2, "HotelBedsV2 GetCancellationCost Request", request.EndPoint);
                }

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    propertyDetails.Logs.AddNew(ThirdParties.HOTELBEDSV2, "HotelBedsV2 GetCancellationCost Response", request.ResponseString);
                }
            }

            return cancellationCostResponse;
        }

        #endregion

        #region ReconciliationReference

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        #endregion
    }

    #region ThirdPartyReference

    public class HotelBedsV2Reference
    {
        public string RateKey { get; set; }
        public string PaymentType { get; set; }
        public string MealBasisCode { get; set; }

        public HotelBedsV2Reference(string rateKey, string paymentType, string mealBasisCode)
        {
            RateKey = rateKey;
            PaymentType = paymentType;
            MealBasisCode = mealBasisCode;
        }

        public static string Create(string rateKey, string paymentType, string mealBasisCode, ISecretKeeper secretKeeper)
        {
            return secretKeeper.Encrypt(JsonConvert.SerializeObject(new HotelBedsV2Reference(rateKey, paymentType, mealBasisCode)));
        }

        public static HotelBedsV2Reference FromEncryptedString(string reference, ISecretKeeper secretKeeper)
        {
            return JsonConvert.DeserializeObject<HotelBedsV2Reference>(secretKeeper.Decrypt(reference));
        }
    }

    #endregion
}