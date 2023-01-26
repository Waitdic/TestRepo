namespace iVectorOne.Suppliers.FastPayHotels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.FastPayHotels.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Models.Property;

    public class FastPayHotels : IThirdParty, ISingleSource
    {
        #region Constructor

        private readonly IFastPayHotelsSettings _settings;
        private readonly ITPSupport _support;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FastPayHotels> _logger;

        public FastPayHotels(IFastPayHotelsSettings settings, ITPSupport support, HttpClient httpClient, ILogger<FastPayHotels> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.FASTPAYHOTELS;

        public bool SupportsRemarks => true;

        public bool SupportsBookingSearch => false;

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
            => Task.FromResult(new ThirdPartyCancellationFeeResult());

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source) => 0;

        public bool RequiresVCard(VirtualCardInfo info, string source) => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source) => false;

        public bool TakeSavingFromCommissionMargin(IThirdPartyAttributeSearch searchDetails) => false;

        #endregion

        #region Access Token

        public static bool IsAccessTokenExpired(string responseString, ref PropertyDetails propertyDetails)
        {
            if (responseString.Equals("Token expired, retry with fresh avail token"))
            {
                propertyDetails.Warnings.AddNew("Access token expired", " Please contact us to regenerate the token");
                return true;
            }

            return false;
        }

        #endregion

        #region Prebook

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            var prebookSuccess = false;
            var prebookResponse = new FastPayHotelsPrebookResponse();
            var prebookUrl = _settings.BookingURL(propertyDetails) + "/prebook";
            var webRequest = new Request();

            try
            {
                var prebookRequest = CreatePrebookRequests(propertyDetails, Guid.NewGuid().ToString());
                string requestString = JsonConvert.SerializeObject(prebookRequest);

                webRequest = CreateWebRequest(prebookUrl, "Prebook", propertyDetails, ContentTypes.Application_json, _settings, requestString);
                await webRequest.Send(_httpClient, _logger);

                if (webRequest.Success)
                {
                    prebookResponse = JsonConvert.DeserializeObject<FastPayHotelsPrebookResponse>(webRequest.ResponseString);
                    prebookSuccess = prebookResponse.result.success;

                    if (prebookSuccess)
                    {
                        // fastpayhotels prebook response doesn't return errata or cancellation terms
                        AppendPrebookTokens(propertyDetails.Rooms, prebookResponse.result.reservationTokens, propertyDetails);
                        propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);
                    }
                }
                else
                {
                    string errorMessage = IsAccessTokenExpired(webRequest.ResponseLog, ref propertyDetails) ? "Access Token has expired" : "Third Party Error, Response was Invalid";
                    throw new Exception("Third Party Error, Response was Invalid", new Exception(errorMessage));
                }
            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", exception.ToString());
                prebookSuccess = false;
            }
            finally
            {
                propertyDetails.AddLog("PreBook", webRequest);
            }

            return prebookSuccess;
        }

        #endregion

        #region Book Restrictions

        public bool ValidateBookRequest(PropertyDetails propertyDetails)
        {
            bool validBookrequest = true;

            // no adult should be younger than 21
            if (propertyDetails.Rooms.Exists(x => x.Passengers.Where(p => (p.PassengerType == PassengerType.Adult || p.PassengerType == PassengerType.Child) && p.Age == 0).Count() > 0))
            {
                validBookrequest = false;
                propertyDetails.Warnings.AddNew("Invalid passenger details", " Please provide age for all passengers");
            }
            else if (propertyDetails.Rooms.Exists(x => x.Passengers.Where(p => p.PassengerType == PassengerType.Adult && p.Age < 21).Count() > 0))
            {
                validBookrequest = false;
                propertyDetails.Warnings.AddNew("Invalid passenger details", "Adult must be 21 or over");
            }

            return validBookrequest;
        }

        #endregion

        #region Book

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            if (!ValidateBookRequest(propertyDetails))
            {
                throw new Exception("Invalid passenger details were provided");
            }

            var bookingUrl = _settings.BookingURL(propertyDetails) + "/book";
            var webRequest = new Request();
            var reference = string.Empty;

            try
            {
                var bookRequest = await CreateBookRequestAsync(propertyDetails, Guid.NewGuid().ToString());
                string requestString = JsonConvert.SerializeObject(bookRequest);

                webRequest = CreateWebRequest(bookingUrl, "Book", propertyDetails, ContentTypes.Application_json, _settings, requestString);
                await webRequest.Send(_httpClient, _logger);

                if (webRequest.Success)
                {
                    var bookingResponse = JsonConvert.DeserializeObject<FastPayHotelsBookResponse>(webRequest.ResponseString);

                    // Only one booking reference is returned - booked as one component.
                    // Single room cancellations not allowed
                    reference = bookingResponse.result.success ? bookingResponse.result.bookingInfo.bookingCode : "Failed";
                }
                else
                {
                    string errorMessage = IsAccessTokenExpired(webRequest.ResponseLog, ref propertyDetails) ? "Access Token has expired" : "Third Party Error, Response was Invalid";
                    throw new Exception("Third Party Error, Response was Invalid", new Exception(errorMessage));
                }
            }
            catch (Exception exception)
            {
                propertyDetails.Warnings.AddNew("Book Exception", exception.ToString());
                reference = "Failed";
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return reference;
        }

        #endregion

        #region Booking Search

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails) => new ThirdPartyBookingSearchResults();

        #endregion

        #region Bookings status update

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails) => new ThirdPartyBookingStatusUpdateResult();

        #endregion

        #region Cancel booking

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var cancelUrl = _settings.BookingURL(propertyDetails) + "/cancel";
            var webRequest = new Request();
            var cancellationResponse = new ThirdPartyCancellationResponse();

            try
            {
                var cancelRequest = CreateCancelRequest(propertyDetails, Guid.NewGuid().ToString());
                string requestString = JsonConvert.SerializeObject(cancelRequest);

                webRequest = CreateWebRequest(cancelUrl, "Cancel", propertyDetails, ContentTypes.Application_json, _settings, requestString);
                await webRequest.Send(_httpClient, _logger);

                if (webRequest.Success)
                {
                    var cancelResponse = JsonConvert.DeserializeObject<FastPayHotelsCancelResponse>(webRequest.ResponseString);
                    if (cancelResponse.success)
                    {
                        cancellationResponse.Success = true;
                        cancellationResponse.TPCancellationReference = propertyDetails.SourceReference; // no cancellation reference will be returned from Fastpay thus booking ref will be returned
                    }
                    else
                    {
                        cancellationResponse.Success = false;
                        cancellationResponse.TPCancellationReference = "failed";
                    }
                }
                else
                {
                    string errorMessage = IsAccessTokenExpired(webRequest.ResponseLog, ref propertyDetails) ? "Access Token has expired" : "Third Party Error, Response was Invalid";
                    throw new Exception("Third Party Error, Response was Invalid", new Exception(errorMessage));
                }
            }
            catch (Exception exception)
            {
                cancellationResponse.Success = false;

                cancellationResponse.TPCancellationReference = "failed";

                propertyDetails.Warnings.AddNew("Cancellation Exception", exception.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", webRequest);
            }

            return cancellationResponse;
        }

        #endregion

        #region Create reconciliation reference

        public string CreateReconciliationReference(string inputReference) => throw new NotImplementedException();

        #endregion

        #region End session

        public void EndSession(PropertyDetails propertyDetails) { }

        #endregion

        #region Helper methods

        public static Request CreateWebRequest(
            string url,
            string logFileName,
            IThirdPartyAttributeSearch tpAttributeSearch,
            string contentType,
            IFastPayHotelsSettings settings,
            string requestString)
        {
            var request = new Request
            {
                EndPoint = url,
                Method = RequestMethod.POST,
                Source = ThirdParties.FASTPAYHOTELS,
                ContentType = contentType,
                CreateLog = true,
                LogFileName = logFileName,
                Accept = "application/json",
                ExtraInfo = tpAttributeSearch
            };

            // Access token needs to be generated per customer every 400 days
            request.Headers.AddNew("Authorization", "Bearer " + settings.AccessToken(tpAttributeSearch));

            request.SetRequest(requestString);
            return request;
        }

        private FastPayHotelsPrebookRequest CreatePrebookRequests(PropertyDetails propertyDetails, string messageId)
        {
            var roomsToPrebook = new List<FastPayHotelsPrebookRequest.Room>();

            foreach (var room in propertyDetails.Rooms)
            {
                roomsToPrebook.Add(
                        new FastPayHotelsPrebookRequest.Room()
                        {
                            availToken = room.ThirdPartyReference,
                            quantity = 1
                        }
                    );
            }

            return new FastPayHotelsPrebookRequest()
            {
                messageID = messageId,
                rooms = roomsToPrebook
            };
        }

        private void AppendPrebookTokens(List<RoomDetails> rooms, List<string> reservationTokens, PropertyDetails propertyDetails)
        {
            for (int i = 0; i < rooms.Count(); ++i)
            {
                rooms[i].ThirdPartyReference += _settings.DelimChar(propertyDetails) + reservationTokens[i]; // Assume that reservation tokens are returned in the order which we have sent availability tokens   
            }
        }

        private FastPayHotelsBookRequest.Customer GetLeadGuestDetails(PropertyDetails propertyDetails)
        {
            return new FastPayHotelsBookRequest.Customer()
            {
                firstName = propertyDetails.LeadGuestFirstName,
                lastName = propertyDetails.LeadGuestLastName,
                email = propertyDetails.LeadGuestEmail,
                phone = propertyDetails.LeadGuestPhone
            };
        }

        private List<FastPayHotelsBookRequest.Pax> GetGuestDetails(RoomDetails roomDetails)
        {
            var guestDetails = new List<FastPayHotelsBookRequest.Pax>();
            foreach (var passenger in roomDetails.Passengers)
            {
                var guest = new FastPayHotelsBookRequest.Pax
                {
                    firstName = passenger.FirstName,
                    lastName = passenger.LastName,
                    age = passenger.Age
                };

                guestDetails.Add(guest);
            }

            return guestDetails;
        }

        private List<FastPayHotelsBookRequest.Room> GetRooms(PropertyDetails propertyDetails)
        {
            var rooms = new List<FastPayHotelsBookRequest.Room>();

            foreach (var roomDetails in propertyDetails.Rooms)
            {
                var room = new FastPayHotelsBookRequest.Room
                {
                    adults = roomDetails.Adults,
                    children = roomDetails.Children + roomDetails.Infants,
                    paxes = GetGuestDetails(roomDetails),
                    comments = new List<string> { roomDetails.SpecialRequest },
                    reservationToken = roomDetails.ThirdPartyReference.Split(_settings.DelimChar(propertyDetails)[0])[1] // 0 - availToken , 1 - reservationToken  
                };

                rooms.Add(room);
            }

            return rooms;
        }

        private async Task<FastPayHotelsBookRequest> CreateBookRequestAsync(PropertyDetails propertyDetails, string messageId)
        {
            return new FastPayHotelsBookRequest()
            {
                messageID = messageId,
                currency = _settings.UseCurrencyCode(propertyDetails) ? await _support.TPCurrencyCodeLookupAsync(propertyDetails.Source, propertyDetails.ISOCurrencyCode) : string.Empty,
                agencyCode = string.IsNullOrEmpty(propertyDetails.BookingReference) ? DateTime.Now.ToString("yyyyMMddhhmmssfff") : propertyDetails.BookingReference,
                comments = "",
                customer = GetLeadGuestDetails(propertyDetails),
                rooms = GetRooms(propertyDetails)
            };
        }

        private FastPayHotelsCancelRequest CreateCancelRequest(PropertyDetails propertyDetails, string messageId)
        {
            return new FastPayHotelsCancelRequest
            {
                messageID = messageId,
                bookingCode = propertyDetails.SourceReference // 0 - bookingToken
            };
        }

        #endregion
    }
}