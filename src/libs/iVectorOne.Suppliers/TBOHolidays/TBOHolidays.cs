namespace iVectorOne.Suppliers.TBOHolidays
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
    using Models;
    using Models.Common;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Suppliers.TBOHolidays.Models.Prebook;
    using iVectorOne.Suppliers.TBOHolidays.Models.Book;
    using iVectorOne.Suppliers.TBOHolidays.Models.Cancel;
    using Newtonsoft.Json;
    using MoreLinq;

    public class TBOHolidays : IThirdParty, ISingleSource
    {
        private readonly ITBOHolidaysSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TBOHolidays> _logger;

        public TBOHolidays(
            ITBOHolidaysSettings settings,
            HttpClient httpClient,
            ILogger<TBOHolidays> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.TBOHOLIDAYS;

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

        public async Task<bool> PreBookAsync(PropertyDetails propertyDetails)
        {
            bool success;
            var priceChanged = false;
            var url = _settings.PrebookURL(propertyDetails);
            var requests = new List<Request>();
            
            try
            {
                var auth = Helper.GetAuth(_settings.User(propertyDetails), _settings.Password(propertyDetails));

                foreach (var room in propertyDetails.Rooms)
                {
                    var requestJson = JsonConvert.SerializeObject(BuildAvailabilityRequest(room));

                    var request = CreateRequest("AvailabilityAndPricing", url, auth);
                    request.SetRequest(requestJson);

                    await request.Send(_httpClient, _logger);
                    requests.Add(request);
                }

                var responses = requests
                    .Select(x => JsonConvert.DeserializeObject<PrebookResponse>(x.ResponseString))
                    .ToList();

                if (responses.All(r => Helper.CheckStatus(r.Status) 
                                       || (r.Status.Code == 200 && r.Status.Description == "The rate has changed.")))
                {
                    // do the price change
                    foreach (var room in propertyDetails.Rooms)
                    {
                        var bookingCode = room.ThirdPartyReference.Split(Helper.Separators, StringSplitOptions.None)[0];

                        var hotelRooms = responses
                            .SelectMany(r => r.HotelResult)
                            .SelectMany(x => x.Rooms)
                            .Where(r => r.BookingCode == bookingCode)
                            .ToList();

                        room.MinimumSellingPrice = hotelRooms
                            .Select(rate => rate.RecommendedSellingRate)
                            .First();

                        var newPrice = hotelRooms
                            .Select(rate => rate.TotalFare)
                            .First();

                        if (newPrice == room.LocalCost) continue;

                        room.LocalCost = newPrice;
                        room.GrossCost = newPrice;
                        priceChanged = true;
                    }

                    if (priceChanged)
                    {
                        propertyDetails.Warnings.AddNew("PriceChanged", "Price was changed");
                    }

                    var newLocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);

                    if (propertyDetails.LocalCost != newLocalCost)
                    {
                        propertyDetails.LocalCost = newLocalCost;
                    }

                    var cancellationList = new Cancellations();
                    foreach (var response in responses)
                    {
                        var cost = propertyDetails.Rooms
                            .First(x => x.ThirdPartyReference.Split(Helper.Separators, StringSplitOptions.None)[0] ==
                                        response.HotelResult.SelectMany(x => x.Rooms).First().BookingCode).LocalCost;

                        var cancelPolicies = response.HotelResult
                            .SelectMany(x => x.Rooms)
                            .SelectMany(c => c.CancelPolicies)
                            .ToList();

                        foreach (var policyNode in cancelPolicies)
                        {
                            var amount = policyNode.ChargeType switch
                            {
                                ChargeType.Fixed => policyNode.CancellationCharge,
                                ChargeType.Percentage => (policyNode.CancellationCharge / 100) * cost,
                                _ => 0
                            };

                            var toDate = cancelPolicies.Any(c => c.FromDate.ToSafeDate() > policyNode.FromDate.ToSafeDate())
                                ? cancelPolicies
                                    .Where(c => c.FromDate.ToSafeDate() > policyNode.FromDate.ToSafeDate())
                                    .Select(x => x.FromDate.ToSafeDate())
                                    .OrderBy(x => x)
                                    .First()
                                    .AddDays(-1)
                                : propertyDetails.ArrivalDate;

                            cancellationList.AddNew(policyNode.FromDate.ToSafeDate(), toDate, amount);
                        }

                        if (response.HotelResult.SelectMany(x => x.Rooms).SelectMany(x => x.Supplements).Any(s => s.Any()))
                        {
                            foreach (var supplements in response.HotelResult.SelectMany(x => x.Rooms).SelectMany(x => x.Supplements))
                            {
                                supplements.ForEach(x => propertyDetails.Errata.AddNew(
                                    "Supplements",
                                    $"Payable at the property: {x.Description} {x.Price} {x.Currency}"));
                            }
                        }
                    }

                    propertyDetails.Cancellations = Cancellations.MergeMultipleCancellationPolicies(cancellationList);

                    if (responses.Any(x => x.HotelResult.Any(hr => hr.RateConditions != Array.Empty<string>())))
                    {
                        var conditions = responses
                            .SelectMany(x => x.HotelResult)
                            .Where(hr => hr.RateConditions != Array.Empty<string>())
                            .Select(r => r.RateConditions)
                            .First();

                        foreach (var condition in conditions)
                        {
                            propertyDetails.Errata.AddNew("Important Information", condition);
                        }
                    }

                    success = true;
                }
                else
                {
                    // failed to prebook
                    propertyDetails.Warnings.AddNew("Prebook Exception", "Prebook was unsuccessful");
                    success = false;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Prebook Exception", ex.ToString());
                success = false;
            }
            finally
            {
                foreach (var request in requests)
                {
                    propertyDetails.AddLog("Prebook Check", request);
                }
            }

            return success;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference;
            var url = _settings.BookingURL(propertyDetails);
            var requests = new List<Request>();
            var requestsDetails = new List<BookingDetailRequest>();

            try
            {
                var auth = Helper.GetAuth(_settings.User(propertyDetails), _settings.Password(propertyDetails));

                foreach (var room in propertyDetails.Rooms)
                {
                    var bookRequest = BuildBookRequestAsync(propertyDetails, room);
                    var bookRequestJson = JsonConvert.SerializeObject(bookRequest);

                    var request = CreateRequest("HotelBook", url, auth);
                    request.SetRequest(bookRequestJson);
                    await request.Send(_httpClient, _logger);

                    requestsDetails.Add(new BookingDetailRequest
                    {
                        BookWebRequest = request,
                        BookRequest = bookRequest,
                    });

                    requests.Add(request);
                }

                foreach (var requestsDetail in requestsDetails)
                {
                    requestsDetail.BookResponse =
                        JsonConvert.DeserializeObject<HotelBookResponse>(requestsDetail.BookWebRequest.ResponseString);
                }

                if (requestsDetails.Any(rd => Helper.CheckStatus(rd.BookResponse.Status)))
                {
                    var detailUrl = _settings.BookingURL(propertyDetails).Replace("/Book", "/BookingDetail");

                    foreach (var requestsDetail in requestsDetails.Where(r => Helper.CheckStatus(r.BookResponse.Status)))
                    {
                        var bookingDetailRequest = BuildBookDetailRequest(
                            string.Empty,
                            requestsDetail.BookRequest.BookingReferenceId,
                            requestsDetail.BookRequest.PaymentMode);

                        var bookingDetailJson = JsonConvert.SerializeObject(bookingDetailRequest);
                        var webRequest = CreateRequest("HotelBookingDetail", detailUrl, auth);
                        webRequest.SetRequest(bookingDetailJson);

                        await webRequest.Send(_httpClient, _logger);
                        requestsDetail.BookDetailWebRequest = webRequest;
                    }
                    
                    requestsDetails.ForEach(b => b.BookingDetailResponse =
                        JsonConvert.DeserializeObject<HotelBookingDetailResponse>(b.BookDetailWebRequest.ResponseString));
                }

                if (requestsDetails.Any(x => Helper.CheckStatus(x.BookResponse.Status) || Helper.CheckStatus(x.BookingDetailResponse.Status)))
                {
                    propertyDetails.SourceReference = requestsDetails.All(x =>
                        Helper.CheckStatus(x.BookResponse.Status) || Helper.CheckStatus(x.BookingDetailResponse.Status))
                        ? "successful"
                        : "failed";

                    var sourceSecondaryReferences = new List<string>();
                    var references = new List<string>();

                    foreach (var requestsDetail in requestsDetails)
                    {
                        var checkBooking = Helper.CheckStatus(requestsDetail.BookResponse.Status)
                                           || (Helper.CheckStatus(requestsDetail.BookingDetailResponse.Status)
                                               && requestsDetail.BookingDetailResponse.BookingDetail.BookingStatus == "Confirmed");

                      
                        if (checkBooking)
                        {
                            var confirmationNo = !string.IsNullOrEmpty(requestsDetail.BookResponse.ConfirmationNumber)
                                ? requestsDetail.BookResponse.ConfirmationNumber
                                : requestsDetail.BookingDetailResponse.BookingDetail.ConfirmationNumber;

                            sourceSecondaryReferences.Add(requestsDetail.BookRequest.ClientReferenceId);
                            references.Add(confirmationNo);
                        }
                        else
                        {
                            sourceSecondaryReferences.Add("failed");
                            references.Add(string.Empty);
                        }
                    }

                    propertyDetails.SourceSecondaryReference = string.Join('|', sourceSecondaryReferences);
                    reference = string.Join('|', references);
                }
                else
                {
                    propertyDetails.Warnings.AddNew("Book Exception", "Failed to confirm booking");
                    reference = "failed";
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                reference = "failed";
            }
            finally
            {
                if (requests.Any())
                {
                    foreach (var request in requests)
                    {
                        propertyDetails.AddLog("Book", request);
                    }
                }

                if (requestsDetails.Any(x => x.BookDetailWebRequest != null))
                {
                    foreach (var requestDetail in requestsDetails.Where(x => x.BookDetailWebRequest != null))
                    {
                        propertyDetails.AddLog("BookingDetail", requestDetail.BookDetailWebRequest);
                    }
                }
                
            }

            return reference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var url = _settings.CancellationURL(propertyDetails);
            var requests = new List<Request>();

            try
            {
                var auth = Helper.GetAuth(_settings.User(propertyDetails), _settings.Password(propertyDetails));
                foreach (var reference in propertyDetails.SourceReference.Split('|'))
                {
                    var cancelRequest = BuildCancelRequest(reference);
                    var cancelRequestJson = JsonConvert.SerializeObject(cancelRequest);

                    var request = CreateRequest("HotelCancel", url, auth);
                    request.SetRequest(cancelRequestJson);
                    await request.Send(_httpClient, _logger);

                    requests.Add(request);
                }

                var responses = requests.Select(r => JsonConvert.DeserializeObject<HotelCancelResponse>(r.ResponseString));

                if (responses.All(r => r.Status.Code == 200 && r.Status.Description == "Cancelled"))
                {
                    thirdPartyCancellationResponse.Success = true;
                    thirdPartyCancellationResponse.TPCancellationReference = propertyDetails.SourceReference;
                }
                else
                {
                    thirdPartyCancellationResponse.Success = false;
                    propertyDetails.Warnings.AddNew(
                        "Cancellation Exception",
                        "Current date is outside of free cancellation period");
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
                thirdPartyCancellationResponse.Success = false;
            }
            finally
            {
                if (requests.Any())
                {
                    foreach (var request in requests)
                    {
                        propertyDetails.AddLog("Cancel", request);
                    }
                }
            }

            return thirdPartyCancellationResponse;
        }

        public Task<ThirdPartyCancellationFeeResult> GetCancellationCostAsync(PropertyDetails propertyDetails)
        {
            return Task.FromResult(new ThirdPartyCancellationFeeResult());
        }

        public ThirdPartyBookingSearchResults BookingSearch(BookingSearchDetails bookingSearchDetails)
        {
            return new ThirdPartyBookingSearchResults();
        }

        public ThirdPartyBookingStatusUpdateResult BookingStatusUpdate(PropertyDetails propertyDetails)
        {
            return new ThirdPartyBookingStatusUpdateResult();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public void EndSession(PropertyDetails propertyDetails)
        {

        }

        private static PrebookRequest BuildAvailabilityRequest(RoomDetails room)
        {
            return new PrebookRequest
            {
                BookingCode = room.ThirdPartyReference.Split("~~~")[0],
                PaymentMode = PaymentMode.Limit.ToString()
            };
        }

        private HotelBookRequest BuildBookRequestAsync(PropertyDetails propertyDetails, RoomDetails room)
        {
            var request = new HotelBookRequest
            {
                BookingCode = room.ThirdPartyReference.Split(Helper.Separators, StringSplitOptions.None)[0],
                ClientReferenceId = $"{DateTime.Now:ddMMyyhhmmssfff}-{01}",
                BookingReferenceId = room.ThirdPartyReference.Split(Helper.Separators, StringSplitOptions.None)[0],
                CustomerDetails = new List<CustomerDetail>
                {
                    new()
                    {
                        CustomerNames = (from passenger in room.Passengers
                            let title = passenger.Title switch
                            {
                                "Mr" or "Miss" or "Mrs" or "Ms" => passenger.Title,
                                _ => "Mr",
                            }
                            select new CustomerName
                            {
                                Title = title,
                                FirstName = passenger.FirstName.Length == 1 ? "TBA" : passenger.FirstName,
                                LastName = passenger.LastName.Length == 1 ? "TBA" : passenger.LastName,
                                Type = passenger.PassengerType is PassengerType.Child or PassengerType.Infant
                                    ? GuestType.Child.ToString()
                                    : GuestType.Adult.ToString()
                            }).ToArray()
                    }
                },
                TotalFare = room.LocalCost,
                EmailId = propertyDetails.LeadGuestEmail,
                PhoneNumber = propertyDetails.LeadGuestPhone,
                BookingType = BookingType.Vouchered.ToString(),
                PaymentMode = PaymentMode.Limit.ToString(),
            };

            return request;
        }

        private HotelBookingDetailRequest BuildBookDetailRequest(
            string confirmationNumber,
            string bookingReferenceId,
            string paymentMode)
        {
            return new HotelBookingDetailRequest
            {
                ConfirmationNumber = confirmationNumber,
                BookingReferenceId = bookingReferenceId,
                PaymentMode = paymentMode
            };
        }

        private static HotelCancelRequest BuildCancelRequest(string reference)
        {
            return new HotelCancelRequest
            {
                ConfirmationNumber = reference
            };
        }

        private static Request CreateRequest(string type, string url, string auth)
        {
            var webRequest = new Request
            {
                Source = ThirdParties.TBOHOLIDAYS,
                LogFileName = type,
                UseGZip = false,
                EndPoint = url,
                CreateLog = true,
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Application_json,
                Accept = "application/json"
            };
            webRequest.Headers.AddNew("Authorization", auth);

            return webRequest;
        }
    } 
}