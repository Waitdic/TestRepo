namespace iVectorOne.CSSuppliers.HotelsProV2
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using iVectorOne.Constants;
    using iVectorOne.CSSuppliers.HotelsProV2.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;

    public class HotelsProV2 : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IHotelsProV2Settings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HotelsProV2> _logger;

        public string Source => ThirdParties.HOTELSPROV2;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        #endregion

        #region Constructors

        public HotelsProV2(IHotelsProV2Settings settings, HttpClient httpClient, ILogger<HotelsProV2> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region PreBook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var provisionRequest = new Request();

            try
            {
                string[] refs = propertyDetails.Rooms.First().ThirdPartyReference.Split('|');
                string code = refs[1];
                string httpGetQuery = $"?search_code={refs[2]}&hotel_code={refs[0]}";
                string endpoint = $"{_settings.HotelAvailabilityURL(propertyDetails)}{httpGetQuery}";

                //'Call the hotel availability request to check if the rooms are still available
                request = await SendGetRequestAsync(propertyDetails, endpoint, "HotelAvailability");

                var searchResponse = JsonConvert.DeserializeObject<ApiResonse<AvailResponse>>(request.ResponseString, GetJsonSerializerSettings());

                //'Check if the rooms returned in the availability response matches the rooms the user selected.
                //  If they dont match rooms are no longer available and fail the prebook
                if (!searchResponse.Results.Select(r => r.RoomCode).Any(roomCode => string.Equals(roomCode, code)))
                {
                    propertyDetails.Warnings.AddNew("PreBook Exception", "No availability for the selected room");
                }

                provisionRequest = await SendPostRequestAsync(
                        propertyDetails,
                        $"{_settings.ProvisionURL(propertyDetails)}{code}",
                        "Prebook", "");

                var prebookResponse = JsonConvert.DeserializeObject<ProvisionResponse>(provisionRequest.ResponseString, GetJsonSerializerSettings());

                if (string.IsNullOrEmpty(prebookResponse.Code))
                {
                    return false;
                }

                //'Store Provision code and Payment Type which is needed for book requests
                propertyDetails.TPRef1 = prebookResponse.Code;

                decimal totalPrice = prebookResponse.Price.ToSafeDecimal();

                bool priceChanged = !decimal.Equals(propertyDetails.LocalCost, totalPrice);

                //'Check for price change
                if (priceChanged)
                {
                    decimal pricePerRoom = totalPrice / propertyDetails.Rooms.Count();
                    foreach (var propertyRoom in propertyDetails.Rooms)
                    {
                        propertyRoom.LocalCost = pricePerRoom;
                        propertyRoom.GrossCost = pricePerRoom;
                    }
                    propertyDetails.LocalCost = totalPrice;
                    propertyDetails.GrossCost = totalPrice;
                }

                if (!string.IsNullOrEmpty(prebookResponse.AdditionalInfo))
                {
                    propertyDetails.Errata.AddNew("Additional Information", prebookResponse.AdditionalInfo);
                }

                propertyDetails.Cancellations = GetCancellations(prebookResponse, propertyDetails);
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.Message);
                return false;
            }
            finally
            {
                propertyDetails.AddLog("Search Pre-Book", request);
                propertyDetails.AddLog("Provision Pre-Book", provisionRequest);
            }

            return true;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();
            const string failed = "failed";
            string reference = "";

            try
            {
                var paxParamList = propertyDetails.Rooms.SelectMany(room =>
                {
                    return room.Passengers.Select(pax =>
                    {
                        return (pax.PassengerType == PassengerType.Adult)
                            ? $"name={room.PropertyRoomBookingID},{pax.FirstName},{pax.LastName},{pax.PassengerType.ToString().ToLower()}"
                            : $"name={room.PropertyRoomBookingID},{pax.FirstName},{pax.LastName},{pax.PassengerType.ToString().ToLower()},{pax.Age}";
                    });
                }).ToList();

                string bookingComments = string.Join(" ", propertyDetails.BookingComments.Select(c => c.Text));
                if (!string.IsNullOrEmpty(bookingComments))
                {
                    paxParamList.Add(bookingComments);
                }

                string sParams = string.Join("&", paxParamList);

                string endpoint = $"{_settings.BookURL(propertyDetails)}{propertyDetails.TPRef1}";
                request = await SendPostRequestAsync(propertyDetails, endpoint, "Book", sParams);

                var oBookResponse = JsonConvert.DeserializeObject<BookResponse>(request.ResponseString, GetJsonSerializerSettings());

                reference = (!string.IsNullOrEmpty(oBookResponse.Code)
                        && string.Equals(oBookResponse.Status.ToLower(), "succeeded"))
                    ? oBookResponse.Code
                    : failed;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.Message);
                reference = failed;
            }
            finally
            {
                propertyDetails.AddLog("Book", request);
            }

            return reference;
        }

        #endregion

        #region Cancellations

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var cancelResponse = new ThirdPartyCancellationResponse();
            const string Failed = "Failed";

            try
            {
                string endpoint = $"{_settings.CancelURL(propertyDetails)}{propertyDetails.SourceReference}";
                request = await SendPostRequestAsync(propertyDetails, endpoint, "Cancel", "");
                var response = JsonConvert.DeserializeObject<CancellationResponse>(request.ResponseString, GetJsonSerializerSettings());
                if (!string.IsNullOrEmpty(response.Code))
                {
                    cancelResponse = new()
                    {
                        Success = true,
                        TPCancellationReference = response.Code,
                        CurrencyCode = response.Currency,
                        Amount = response.ChargeAmount.ToSafeDecimal()
                    };
                }
                else
                {
                    cancelResponse = new()
                    {
                        Success = false,
                        TPCancellationReference = Failed
                    };
                }
            }
            catch (Exception ex)
            {
                cancelResponse = new()
                {
                    Success = false,
                    TPCancellationReference = Failed
                };
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.Message);
            }
            finally
            {
                propertyDetails.AddLog("Cancel", request);
            }

            return cancelResponse;
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

        #endregion

        #region Other Methods

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.OffsetCancellationDays(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            return false;
        }

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        #endregion

        #region Helpers

        private Cancellations GetCancellations(ProvisionResponse preBookResponse, PropertyDetails propertyDetails)
        {
            var cancellations = new Cancellations();
            var previousStartDate = DateTime.MinValue;

            foreach (var cancellationPolicy in preBookResponse.Policies)
            {
                decimal multiplier = cancellationPolicy.Ratio.ToSafeDecimal();
                int remainingDays = cancellationPolicy.DaysRemaining.ToSafeInt();
                var policyStartDate = propertyDetails.ArrivalDate;
                var policyEndDate = new DateTime(2099, 1, 1);

                if (remainingDays != 0)
                {
                    policyStartDate = propertyDetails.ArrivalDate.AddDays(-remainingDays);
                    policyEndDate = (previousStartDate != DateTime.MinValue)
                        ? previousStartDate.AddDays(-1)
                        : propertyDetails.DepartureDate;
                }

                decimal feeAmount = propertyDetails.LocalCost * multiplier;

                cancellations.AddNew(policyStartDate, policyEndDate, feeAmount);
                previousStartDate = policyStartDate;
            }

            return cancellations;
        }

        private async Task<Request> SendGetRequestAsync(PropertyDetails propertyDetails, string endpoint, string logFileName)
        {
            var request = new Request
            {
                EndPoint = endpoint,
                Method = RequestMethod.GET,
                Source = Source,
                LogFileName = logFileName,
                AuthenticationMode = AuthenticationMode.Basic,
                UserName = _settings.UserName(propertyDetails),
                Password = _settings.Password(propertyDetails),
                ExtraInfo = propertyDetails,
                CreateLog = true,
                ContentType = ContentTypes.Application_x_www_form_urlencoded,
            };
            await request.Send(_httpClient, _logger);

            return request;
        }

        private async Task<Request> SendPostRequestAsync(PropertyDetails propertyDetails, string endpoint, string logFileName, string body)
        {
            var request = new Request
            {
                EndPoint = endpoint,
                Method = RequestMethod.POST,
                Source = Source,
                LogFileName = logFileName,
                AuthenticationMode = AuthenticationMode.Basic,
                UserName = _settings.UserName(propertyDetails),
                Password = _settings.Password(propertyDetails),
                ExtraInfo = propertyDetails,
                CreateLog = true,
                ContentType = ContentTypes.Application_x_www_form_urlencoded,
            };
            request.SetRequest(body);
            await request.Send(_httpClient, _logger);

            return request;
        }

        public static JsonSerializerSettings GetJsonSerializerSettings() =>
           new() { DateFormatString = Constant.DateFormat };

        #endregion
    }
}