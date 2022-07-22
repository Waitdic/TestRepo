namespace iVectorOne.CSSuppliers.HotelBedsV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Security;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using System.Threading.Tasks;

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

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
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

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool prebookSuccess = true;
            var request = new Request();
            var response = new HotelBedsV2CheckRatesResponse();

            try
            {
                var checkRatesRequest = new HotelBedsV2CheckRatesRequest()
                {
                    language = _settings.LanguageCode(propertyDetails),
                };

                var rooms = new List<HotelBedsV2CheckRatesRequest.Room>();

                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    var reference = HotelBedsV2Reference.FromEncryptedString(roomDetails.ThirdPartyReference, _secretKeeper);
                    var room = new HotelBedsV2CheckRatesRequest.Room()
                    {
                        rateKey = reference.RateKey,
                    };
                    rooms.Add(room);
                }
                checkRatesRequest.rooms = rooms.ToArray();

                request.EndPoint = _settings.CheckRatesURL(propertyDetails);
                request.Method = RequestMethod.POST;
                request.Source = ThirdParties.HOTELBEDSV2;
                request.ContentType = ContentTypes.Application_json;
                request.UseGZip = _settings.UseGZip(propertyDetails);
                request.CreateLog = true;
                request.LogFileName = "Prebook";
                request.Accept = "application/json";

                string requestString = JsonConvert.SerializeObject(checkRatesRequest);
                requestString = requestString.Replace("\"upselling\":null,", "");
                request.SetRequest(requestString);

                request.Headers.AddNew("Api-key", _settings.User(propertyDetails));
                request.Headers.AddNew("X-Signature", HotelBedsV2Search.GetSignature(_settings.User(propertyDetails), _settings.Password(propertyDetails)));

                await request.Send(_httpClient, _logger);

                if (!string.IsNullOrWhiteSpace(request.ResponseString))
                {
                    response = JsonConvert.DeserializeObject<HotelBedsV2CheckRatesResponse>(request.ResponseString);
                }
                else
                {
                    throw new Exception("Third Party Error, Response was Invalid", new Exception("Third Party Error, Response was Invalid"));
                }

                var processedRooms = new List<int>();

                foreach (var room in response.hotel.rooms)
                {
                    foreach (var rate in room.rates)
                    {
                        bool rateUsed = false;
                        foreach (var roomDetails in propertyDetails.Rooms
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
                                roomDetails.LocalCost = rate.net.ToSafeDecimal();
                                roomDetails.GrossCost = rate.net.ToSafeDecimal();

                                if (!string.IsNullOrWhiteSpace(rate.rateComments))
                                {
                                    propertyDetails.Errata.AddNew("Important Information", rate.rateComments);
                                }

                                var orderedCancellations = rate.cancellationPolicies.OrderBy(o => o.from.Date).ToList();
                                for (var i = 0; i < orderedCancellations.Count; i++)
                                {
                                    var cancellationCharge = orderedCancellations[i];
                                    var endDate = (i + 1) < orderedCancellations.Count ?
                                        orderedCancellations[i + 1].from.Date.AddDays(-1) :
                                        propertyDetails.ArrivalDate;
                                    propertyDetails.Cancellations.AddNew(cancellationCharge.from.Date, endDate, cancellationCharge.amount.ToSafeDecimal());
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
                propertyDetails.AddLog("PreBook", request);
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

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
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
                    remark = propertyDetails.Rooms.Where(x => !string.IsNullOrEmpty(x.SpecialRequest)).Any() ? 
                     string.Join("\n", propertyDetails.Rooms.Select(x => x.SpecialRequest)):
                     "", 
                    clientReference = propertyDetails.BookingReference
                };

                for (int i = 0; i <= totalRooms - 1; i++)
                {
                    var room = propertyDetails.Rooms.ElementAt(i);
                    var hotelBedsV2Reference = HotelBedsV2Reference.FromEncryptedString(room.ThirdPartyReference, _secretKeeper);
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
                            roomId = "1",
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

                webRequest.Source = ThirdParties.HOTELBEDSV2;
                webRequest.Method = RequestMethod.POST;
                webRequest.EndPoint = _settings.BookingURL(propertyDetails);

                if (RequiresVCard)
                {
                    webRequest.EndPoint = _settings.SecureBookingURL(propertyDetails);
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
                webRequest.UseGZip = _settings.UseGZip(propertyDetails);
                webRequest.CreateLog = true;
                webRequest.LogFileName = "Book";
                webRequest.Accept = "application/json";

                string requestString = JsonConvert.SerializeObject(request);
                requestString = requestString.Replace(",\"paymentData\":null", "");
                requestString = requestString.Replace(",\"clientReference\":null", "");
                requestString = requestString.Replace(",\"remark\":null", "");
                webRequest.SetRequest(requestString);

                webRequest.Headers.AddNew("Api-key", _settings.User(propertyDetails));
                webRequest.Headers.AddNew("X-Signature", HotelBedsV2Search.GetSignature(_settings.User(propertyDetails), _settings.Password(propertyDetails)));

                await webRequest.Send(_httpClient, _logger);

                var response = JsonConvert.DeserializeObject<HotelBedsV2CreateBookingResponse>(webRequest.ResponseString);

                if (!string.IsNullOrEmpty(response.booking.reference))
                {
                    reference = response.booking.reference;
                }
                else
                {
                    reference = "failed";
                }
            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Book Exception", exception.ToString());
                reference = "failed";
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return reference;
        }

        #endregion

        #region Cancel

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var cancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                IThirdPartyAttributeSearch searchDetails = new SearchDetails
                {
                    ThirdPartyConfigurations = propertyDetails.ThirdPartyConfigurations,
                };

                request.Method = RequestMethod.DELETE;
                request.EndPoint = _settings.CancellationURL(searchDetails);
                request.Source = ThirdParties.HOTELBEDSV2;
                request.ContentType = ContentTypes.Application_json;
                request.UseGZip = _settings.UseGZip(searchDetails);
                request.CreateLog = true;
                request.LogFileName = "Cancel";
                request.Accept = "application/json";

                request.EndPoint = request.EndPoint.Replace("{{bookingReference}}", propertyDetails.SourceReference);

                request.Headers.AddNew("Api-key", _settings.User(searchDetails));
                request.Headers.AddNew("X-Signature", HotelBedsV2Search.GetSignature(_settings.User(searchDetails), _settings.Password(searchDetails)));

                await request.Send(_httpClient, _logger);

                var response = JsonConvert.DeserializeObject<HotelBedsV2CancellationResponse>(request.ResponseString);

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
                propertyDetails.AddLog("Cancellation", request);
            }

            return cancellationResponse;
        }

        #endregion

        #region GetCancellationCosts

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var cancellationCostResponse = new ThirdPartyCancellationFeeResult();

            try
            {
                request.Method = RequestMethod.DELETE;
                request.EndPoint = _settings.CancellationURL(propertyDetails);
                request.Source = ThirdParties.HOTELBEDSV2;
                request.ContentType = ContentTypes.Application_json;
                request.UseGZip = _settings.UseGZip(propertyDetails);
                request.CreateLog = true;
                request.LogFileName = "Cancel";
                request.Accept = "application/json";

                request.EndPoint = request.EndPoint.Replace("{{bookingReference}}", propertyDetails.SourceReference);
                request.EndPoint = request.EndPoint.Replace("cancellationFlag=CANCELLATION", "cancellationFlag=SIMULATION");

                request.Headers.AddNew("Api-key", _settings.User(propertyDetails));
                request.Headers.AddNew("X-Signature", HotelBedsV2Search.GetSignature(_settings.User(propertyDetails), _settings.Password(propertyDetails)));

                await request.Send(_httpClient, _logger);

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
                propertyDetails.AddLog("GetCancellationCost", request);
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