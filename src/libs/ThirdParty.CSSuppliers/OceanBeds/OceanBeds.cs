namespace ThirdParty.CSSuppliers.OceanBeds
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.CSSuppliers.OceanBeds.Models;
    using ThirdParty.CSSuppliers.OceanBeds.Models.Common;
    using static OceanBedsHelper;
    using Status = Models.Common.Status;

    public class OceanBeds : IThirdParty, ISingleSource
    {
        private readonly IOceanBedsSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<OceanBeds> _logger;

        public OceanBeds(IOceanBedsSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<OceanBeds> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.OCEANBEDS;

        public bool SupportsRemarks => false;

        public bool SupportsBookingSearch => false;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
            => _settings.AllowCancellations(searchDetails);

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
            => 0;

        public bool RequiresVCard(VirtualCardInfo info, string source)
            => false;

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            const string requestType = "GetPropertyAvailability";
            string endpoint = _settings.SearchEndPoint(propertyDetails);
            bool success = false;
            var webRequest = new Request();

            try
            {
                var propertyAvailabilityRequest = BuildPropertyAvailabilityRequest(new OceanBedsPropertyDetails(propertyDetails), propertyDetails, _settings);

                webRequest = await SendRequestAsync(_serializer.Serialize(propertyAvailabilityRequest), propertyDetails, endpoint, requestType);
                var propertyAvailabilityRs = _serializer.DeSerialize<AvailabilityRS>(webRequest.ResponseXML);

                success = CheckSuccess(propertyAvailabilityRs.Status) && PreBookPriceCheck(propertyAvailabilityRs, propertyDetails);

                propertyDetails.Errata.AddNew("Booking Remark", propertyAvailabilityRs.Response[0].CancellationText);

            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Ocean Beds Prebook Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Prebook", webRequest);
            }

            return success;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference;
            var bookWebRequest = new Request();

            try
            {
                var bookingMultipleRq = BuildBookingRequest(propertyDetails);
                bookWebRequest = await SendRequestAsync(
                    _serializer.Serialize(bookingMultipleRq),
                    propertyDetails,
                    _settings.BookEndPoint(propertyDetails),
                    "Book");

                var bookingResponse = _serializer.DeSerialize<BookingMultipleRS>(bookWebRequest.ResponseXML);

                if (!CheckSuccess(bookingResponse.Status))
                    throw new Exception();

                reference = bookingResponse.Response;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.Message);
                reference = "failed";
            }
            finally
            {
                propertyDetails.AddLog("Book", bookWebRequest);
            }

            return reference;
        }

        #region Cancellation

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            const string requestType = "ConfirmCancellation";
            var cancellationResponse = new ThirdPartyCancellationResponse();
            var webRequest = new Request();

            try
            {
                //We need to get the Cancellation Token inorder to cancel
                await CancellationCostsAsync(propertyDetails);
                string cancellationKey = propertyDetails.TPRef1;

                if (string.IsNullOrWhiteSpace(cancellationKey))
                    throw new Exception("No Cancellation Key");

                var endpoint = _settings.ConfirmCancelEndPoint(propertyDetails);
                var credentials = Credentials(propertyDetails, _settings);

                var confirmCancellationRq = BuildConfirmCancellation(cancellationKey.ToSafeInt(), credentials);
                var request = _serializer.Serialize(confirmCancellationRq);

                webRequest = await SendRequestAsync(request, propertyDetails, endpoint, requestType);
                var confirmCancellationRs = _serializer.DeSerialize<ConfirmCancellationRS>(webRequest.ResponseXML);
                var bookingReference = confirmCancellationRs.Response[0].BookingRef;

                cancellationResponse.Success = CheckSuccess(confirmCancellationRs.Status);
                cancellationResponse.Amount = confirmCancellationRs.Response[0].CancellationCharge.ToSafeDecimal();
                cancellationResponse.TPCancellationReference = $"{bookingReference}|{cancellationKey}";
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Ocean Beds Cancel Booking Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Cancel Booking", webRequest);
            }

            return cancellationResponse;
        }

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
            => await CancellationCostsAsync(propertyDetails);

        private async Task<ThirdPartyCancellationFeeResult> CancellationCostsAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            var cancellationFeeResult = new ThirdPartyCancellationFeeResult();
            const string requestType = "GetBookingCancellation";

            try
            {
                var endpoint = _settings.GetCancellationEndPoint(propertyDetails);
                var credentials = Credentials(propertyDetails, _settings);

                var cancellationRequest = BuildBookingCancellation(propertyDetails.SourceReference, credentials);
                var request = _serializer.Serialize(cancellationRequest);

                webRequest = await SendRequestAsync(request, propertyDetails, endpoint, requestType);
                var bookingCancellationRs = _serializer.DeSerialize<BookingCancellationRS>(webRequest.ResponseXML);

                cancellationFeeResult.Amount = bookingCancellationRs.Response[0].CancellationCharge.ToSafeDecimal();
                cancellationFeeResult.Success = CheckSuccess(bookingCancellationRs.Status);
                cancellationFeeResult.CurrencyCode = _settings.Currency(propertyDetails);

                propertyDetails.TPRef1 = bookingCancellationRs.Response[0].Key.ToSafeString();

            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Ocean Beds Cancellation Cost Exception", ex.ToString());
            }
            finally
            {
                propertyDetails.AddLog("Cancellation Cost", webRequest);
            }

            return cancellationFeeResult;
        }

        #endregion

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails) => new();

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails) => new();

        public string CreateReconciliationReference(string inputReference)
            => string.Empty;

        public void EndSession(PropertyDetails propertyDetails)
        {
            throw new NotImplementedException();
        }

        #region Helper Functions

        public bool PreBookPriceCheck(AvailabilityRS oceanBedsAvailabilityRs, PropertyDetails propertyDetails)
        {
            foreach (var room in oceanBedsAvailabilityRs.Response)
            {
                foreach (var roomList in room.RoomList)
                {
                    var searchRoomCode = roomList.Code;

                    foreach (var roomDetail in propertyDetails.Rooms)
                    {
                        var preBookRoomCode = roomDetail.RoomTypeCode.Split('|')[1];
                        var searchCost = Math.Round(roomDetail.GrossCost, 2);

                        if (searchRoomCode.Equals(preBookRoomCode))
                        {
                            var preBookCost = Math.Round(roomList.NetPrice, 2);

                            if (searchCost.Equals(preBookCost))
                                continue;

                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static ConfirmCancellationRQ BuildConfirmCancellation(int cancelKey, Credential credentials)
        {
            return new ConfirmCancellationRQ
            {
                Credential = credentials,
                Key = cancelKey
            };
        }

        private async Task<Request> SendRequestAsync(
            XmlDocument request,
            IThirdPartyAttributeSearch thirdPartyAttributeSearch,
            string endpoint,
            string requestType)
        {
            var webRequest = new Request
            {
                EndPoint = endpoint,
                Method = RequestMethod.POST,
                Source = ThirdParties.OCEANBEDS,
                LogFileName = requestType,
                CreateLog = true,
                UseGZip = _settings.UseGzip(thirdPartyAttributeSearch).ToSafeBoolean(),
                SOAP = false
            };

            webRequest.SetRequest(request);
            await webRequest.Send(_httpClient, _logger);

            return webRequest;
        }

        private BookingMultipleRQ BuildBookingRequest(PropertyDetails propertyDetails)
        {
            string phoneNumber = propertyDetails.LeadGuestPhone;
            string emailAddress = propertyDetails.LeadGuestEmail;

            if (string.IsNullOrEmpty(phoneNumber))
            {
                phoneNumber = _settings.Telephone(propertyDetails);
            }

            if (string.IsNullOrEmpty(emailAddress))
            {
                emailAddress = _settings.DefaultEmail(propertyDetails);
            }

            var bookingMultipleRq = new BookingMultipleRQ
            {
                Credential = Credentials(propertyDetails, _settings),
                Title = propertyDetails.LeadGuestTitle,
                FirstName = propertyDetails.LeadGuestFirstName,
                LastName = propertyDetails.LeadGuestLastName,
                Email = emailAddress,
                ContactNo = phoneNumber,
                BookingReference = propertyDetails.BookingReference
            };

            foreach (var room in propertyDetails.Rooms)
            {
                var booking = new Booking
                {
                    CheckInDate = propertyDetails.ArrivalDate.ToDateString(),
                    CheckOutDate = propertyDetails.DepartureDate.ToDateString(),
                    Adults = room.Adults,
                    Children = room.Children,
                    Infants = room.Infants,
                    RoomId = room.RoomTypeCode.Split('|')[0],
                    NetPrice = Math.Round(room.GrossCost, 2).ToSafeString(),
                    SpecialRequest = "",
                    BookingType = "BookNow"
                };

                bookingMultipleRq.BookingList.Add(booking);
            }

            return bookingMultipleRq;
        }

        private static BookingCancellationRQ BuildBookingCancellation(string sourceReference, Credential credential)
        {
            var bookingCancellationRq = new BookingCancellationRQ
            {
                Credential = credential,
                MasterBookingId = sourceReference
            };

            return bookingCancellationRq;
        }

        public bool CheckSuccess(Status status) => status.Message.ToLower().Equals("success");

        #endregion
    }
}