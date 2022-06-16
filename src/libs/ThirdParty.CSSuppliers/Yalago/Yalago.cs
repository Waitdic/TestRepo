namespace ThirdParty.CSSuppliers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.Models.Yalago;
    using ThirdParty.Interfaces;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;

    public class Yalago : IThirdParty, ISingleSource
    {
        #region Properties

        private readonly IYalagoSettings _settings;
        private readonly ITPSupport _support;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Yalago> _logger;

        public string Source => ThirdParties.YALAGO;

        public bool SupportsRemarks => true;

        public bool SupportsLiveCancellation(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return _settings.AllowCancellations(searchDetails);
        }

        public bool RequiresVCard(VirtualCardInfo info, string source)
        {
            throw new NotImplementedException();
        }

        public int OffsetCancellationDays(IThirdPartyAttributeSearch searchDetails, string source)
        {
            return 0;
        }

        #endregion

        #region Constructors

        public Yalago(IYalagoSettings settings, ITPSupport support, HttpClient httpClient, ILogger<Yalago> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        public bool SupportsBookingSearch => throw new NotImplementedException();

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var cancellationResponse = new ThirdPartyCancellationResponse();

            var thirdPartyCancellationFeeResult = await GetCancellationCostAsync(propertyDetails);

            var expectedCharge = new YalagoCancellationRequest.ExpectedCharge();
            var charge = new YalagoCancellationRequest.Charge()
            {
                Amount = thirdPartyCancellationFeeResult.Amount,
                Currency = thirdPartyCancellationFeeResult.CurrencyCode
            };

            expectedCharge.charge = charge;

            var cancelRequest = new YalagoCancellationRequest()
            {
                BookingRef = propertyDetails.SourceReference,
                expectedCharge = expectedCharge
            };

            string cancelRequestString = JsonConvert.SerializeObject(cancelRequest);

            try
            {
                request = BuildRequest("Cancel", cancelRequestString, propertyDetails, propertyDetails, _settings.CancellationURL(propertyDetails));

                await request.Send(_httpClient, _logger);

                var response = JsonConvert.DeserializeObject<YalagoCancellationResponse>(request.ResponseString);
                if (thirdPartyCancellationFeeResult.Success && response.Status == "1")
                {
                    cancellationResponse.Success = response.Status == "1";
                    cancellationResponse.CurrencyCode = thirdPartyCancellationFeeResult.CurrencyCode;
                    cancellationResponse.Amount = thirdPartyCancellationFeeResult.Amount;
                    cancellationResponse.TPCancellationReference = response.BookingRef;
                }
                else
                {
                    cancellationResponse.Success = false;
                    cancellationResponse.TPCancellationReference = "failed";
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
                propertyDetails.AddLog("Cancellation", request);
            }

            return cancellationResponse;
        }

        public string CreateReconciliationReference(string inputReference)
        {
            throw new NotImplementedException();
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        public async Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            var request = new Request();
            var preCancelRequest = new YalagoPreCancelRequest()
            {
                BookingRef = propertyDetails.SourceReference,
                GetTaxBreakdown = false
            };
            string preCancelRequestString = JsonConvert.SerializeObject(preCancelRequest);
            var cancellationCostResponse = new ThirdPartyCancellationFeeResult();

            try
            {
                request = BuildRequest("PreCancel", preCancelRequestString, propertyDetails, propertyDetails, _settings.PreCancellationURL(propertyDetails));
                await request.Send(_httpClient, _logger);

                var response = JsonConvert.DeserializeObject<YalagoPreCancelResponse>(request.ResponseString);

                if (response.IsCancellable)
                {
                    cancellationCostResponse.Amount = response.charge.charge.Amount;
                    cancellationCostResponse.CurrencyCode = response.cancellationPolicyStatic[0].cancellationCharges[0].charge.Currency;
                    cancellationCostResponse.Success = response.IsCancellable;
                }
                else
                {
                    cancellationCostResponse.Success = response.IsCancellable;
                }
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

        private Request BuildRequest(string requestType, string requestString, IThirdPartyAttributeSearch searchDetails, PropertyDetails propertyDetails, string url)
        {
            var request = new Request
            {
                Source = ThirdParties.YALAGO,
                Method = RequestMethod.POST,
                EndPoint = url,
                ContentType = "application/json",
                UseGZip = _settings.UseGZip(searchDetails),
                CreateLog = true,
                LogFileName = requestType,
                Accept = "application/gzip",
                TimeoutInSeconds = 100,
                KeepAlive = true
            };

            request.SetRequest(requestString);

            request.Headers.Add("X-Api-Key", _settings.APIKey(searchDetails));

            return request;
        }

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool prebookSuccess = true;
            var response = new YalagoPreBookResponse();
            var request = new Request();
            try
            {
                string sourceMarket = await _support.TPCountryCodeLookupAsync(ThirdParties.YALAGO, propertyDetails.SellingCountry, propertyDetails.SubscriptionID);
                if (string.IsNullOrEmpty(sourceMarket))
                {
                    sourceMarket = _settings.SourceMarket(propertyDetails);
                }

                var opaqueSearch = propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[4].ToSafeBoolean();
                var getPackagePrice = opaqueSearch && _settings.ReturnOpaqueRates(propertyDetails) && propertyDetails.OpaqueRates;

                var preBookRequest = new YalagoPreBookRequest()
                {
                    Culture = _settings.LanguageCode(propertyDetails),
                    CheckInDate = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                    CheckOutDate = propertyDetails.DepartureDate.ToString("yyyy-MM-dd"),
                    LocationId = propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[2].ToSafeInt(),
                    EstablishmentId = propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[0].ToSafeInt(),
                    SourceMarket = sourceMarket,
                    GetLocalCharges = true,
                    GetPackagePrice = getPackagePrice
                };

                var rooms = new List<YalagoPreBookRequest.Room>();

                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    var room = new YalagoPreBookRequest.Room()
                    {
                        BoardCode = roomDetails.ThirdPartyReference.Split('|')[1],
                        RoomCode = roomDetails.ThirdPartyReference.Split('|')[3],
                        Adults = roomDetails.Adults,
                        ChildAges = roomDetails.ChildAndInfantAges.ToArray()
                    };

                    rooms.Add(room);
                }

                preBookRequest.Rooms = rooms.ToArray();

                string requestString = JsonConvert.SerializeObject(preBookRequest);

                request = BuildRequest("PreBook", requestString, propertyDetails, propertyDetails, _settings.PrebookURL(propertyDetails));

                await request.Send(_httpClient, _logger);

                response = JsonConvert.DeserializeObject<YalagoPreBookResponse>(request.ResponseString);

                var processedRooms = new List<int>();

                if (response?.establishment?.Rooms is null || !response.establishment.Rooms.Any())
                {
                    propertyDetails.Warnings.AddNew("Prebook Response Error", "No room details found on prebook response");
                    return false;
                }

                foreach (var room in response.establishment.Rooms.Where(r => r?.Boards is object))
                {
                    foreach (var board in room.Boards.Where(b => b is object))
                    {
                        foreach (var roomDetails in propertyDetails.Rooms
                            .Where(o =>
                                !processedRooms.Contains(o.PropertyRoomBookingID) &&
                                propertyDetails.Rooms.IndexOf(o) == board.RequestedRoomIndex.ToSafeInt() &&
                                o.RoomType.ToSafeString().ToLower() == room.Description.ToSafeString().ToLower() &&
                                o.ThirdPartyReference.Split('|')[3].ToLower() == room.Code.ToLower() &&
                                o.ThirdPartyReference.Split('|')[5] == board.Type.ToSafeString()))

                        {
                            foreach (var localCharge in board.LocalCharges)
                            {
                                propertyDetails.Errata.Add(new Erratum("Local Charge", localCharge.Amount.Currency.ToString() + localCharge.Amount.Amount.ToString()));
                            }

                            roomDetails.LocalCost = board.netCost.Amount.ToSafeDecimal();
                            roomDetails.GrossCost = board.netCost.Amount.ToSafeDecimal();

                            if (response.InfoItems != null)
                            {
                                var importantInfo = new HashSet<string>(
                                    response.InfoItems
                                        .Where(i => !string.IsNullOrWhiteSpace(i?.Description))
                                        .Select(i => i.Description));

                                propertyDetails.Errata.AddRange(importantInfo.Select(description =>
                                    new Erratum("ImportantInformation", description)));
                            }

                            var orderedCancellations = board.cancellationPolicy.CancellationCharges.OrderByDescending(o => o.ExpiryDate.Date).ToList();
                            for (var i = 0; i < orderedCancellations.Count; i++)
                            {
                                var cancellationCharge = orderedCancellations[i];
                                var startDate = (i + 1) < orderedCancellations.Count ?
                                    orderedCancellations[i + 1].ExpiryDate.Date.AddDays(1) : DateTime.Now.Date;
                                propertyDetails.Cancellations.AddNew(startDate, cancellationCharge.ExpiryDate.Date, cancellationCharge.charge.Amount.ToSafeDecimal());
                            }

                            processedRooms.Add(roomDetails.PropertyRoomBookingID);
                        }
                    }
                }

                propertyDetails.Cancellations.Solidify(SolidifyType.Sum);

                prebookSuccess = response.establishment != null;
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                prebookSuccess = false;
            }
            finally
            {
                propertyDetails.AddLog("PreBook", request);
            }

            return prebookSuccess;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference = string.Empty;
            bool opaqueSearch = false;
            int i = 1;
            var bookingRequest = new Request();

            try
            {
                var totalRooms = propertyDetails.Rooms.Count;
                var roomsList = new List<YalagoCreateBookingRequest.Room>();

                foreach (var room in propertyDetails.Rooms)
                {
                    var guestList = new List<YalagoCreateBookingRequest.Guest>();

                    foreach (var passenger in room.Passengers)
                    {
                        if (passenger.Age == 0 && passenger.PassengerType != PassengerType.Infant)
                        {
                            passenger.Age = 25;
                        }

                        var guest = new YalagoCreateBookingRequest.Guest()
                        {
                            Age = passenger.Age,
                            FirstName = passenger.FirstName,
                            LastName = passenger.LastName,
                            Title = passenger.Title
                        };
                        guestList.Add(guest);
                    }
                    var expectedNetCost = new YalagoCreateBookingRequest.ExpectedNetCost
                    {
                        Amount = Math.Round(room.GrossCost.ToSafeDecimal(), 2),
                        Currency = propertyDetails.ISOCurrencyCode
                    };

                    var requestRoom = new YalagoCreateBookingRequest.Room()
                    {
                        Guests = guestList.ToArray(),
                        RoomCode = room.ThirdPartyReference.Split('|')[3],
                        BoardCode = room.ThirdPartyReference.Split('|')[1],
                        AffiliateRoomRef = ("room" + i++).ToSafeString(),
                        SpecialRequests = room.SpecialRequest, 
                        ExpectedNetCost = expectedNetCost
                    };

                    roomsList.Add(requestRoom);

                    opaqueSearch = room.ThirdPartyReference.Split('|')[4].ToSafeBoolean();
                }

                var getPackagePrice = opaqueSearch && _settings.ReturnOpaqueRates(propertyDetails) && propertyDetails.OpaqueRates; ;

                var contactDetails = new YalagoCreateBookingRequest.ContactDetails()
                {
                    Title = propertyDetails.LeadGuestTitle,
                    Address1 = propertyDetails.LeadGuestAddress1,
                    Address2 = propertyDetails.LeadGuestAddress2,
                    FirstName = propertyDetails.LeadGuestFirstName,
                    LastName = propertyDetails.LeadGuestLastName,
                    PostCode = propertyDetails.LeadGuestPostcode
                };

                string sourceMarket = await _support.TPCountryCodeLookupAsync(ThirdParties.YALAGO, propertyDetails.SellingCountry, propertyDetails.SubscriptionID);
                if (string.IsNullOrEmpty(sourceMarket))
                {
                    sourceMarket = _settings.SourceMarket(propertyDetails);
                }

                var request = new YalagoCreateBookingRequest()
                {
                    AffiliateRef = propertyDetails.BookingReference,
                    CheckInDate = propertyDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                    CheckOutDate = propertyDetails.DepartureDate.ToString("yyyy-MM-dd"),
                    EstablishmentId = propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[0].ToSafeInt(),
                    Culture = _settings.LanguageCode(propertyDetails),
                    GetPackagePrice = getPackagePrice,
                    GetTaxBreakdown = true,
                    GetLocalCharges = true,
                    Rooms = roomsList.ToArray(),
                    contactDetails = contactDetails,
                    SourceMarket = sourceMarket
                };

                string requestString = JsonConvert.SerializeObject(request);

                bookingRequest = BuildRequest("Book", requestString, propertyDetails, propertyDetails, _settings.BookingURL(propertyDetails));

                await bookingRequest.Send(_httpClient, _logger);

                var response = new YalagoCreateBookingResponse();

                response = JsonConvert.DeserializeObject<YalagoCreateBookingResponse>(bookingRequest.ResponseString);

                if (response != null && response.Bookings != null && !string.IsNullOrEmpty(response.BookingRef))
                {
                    reference = response.BookingRef;
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
                propertyDetails.AddLog("Book", bookingRequest);
            }

            return reference;
        }
    }
}