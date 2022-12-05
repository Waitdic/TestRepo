namespace iVectorOne.Suppliers.TBOHolidays
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.Common;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Suppliers.TBOHolidays.Models.Prebook;
    using iVectorOne.Suppliers.TBOHolidays.Models.Book;
    using iVectorOne.Suppliers.TBOHolidays.Models.Cancel;
    using Newtonsoft.Json;
    using MoreLinq;

    public class TBOHolidays : IThirdParty, ISingleSource
    {
        private readonly ITBOHolidaysSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<TBOHolidays> _logger;

        public TBOHolidays(
            ITBOHolidaysSettings settings,
            ITPSupport support,
            ISerializer serializer,
            HttpClient httpClient,
            ILogger<TBOHolidays> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
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
            var success = false;
            var priceChanged = false;
            var url = /*_settings.PrebookURL(propertyDetails)*/ "https://api.tbotechnology.in/TBOHolidays_HotelAPI/PreBook";
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

                if (responses.All(r => r.Status.Code == 200 && r.Status.Description == "Successful"))
                {
                    // do the price change
                    foreach (var room in propertyDetails.Rooms)
                    {
                        var bookingCode = room.ThirdPartyReference.Split(Helper.Separators, StringSplitOptions.None)[0];
                        var newPrice = responses
                            .SelectMany(r => r.HotelResult)
                            .SelectMany(x => x.Rooms)
                            .Where(r => r.BookingCode == bookingCode)
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
                    //var priceDetails = string.Empty;

                    //// grab the price details and store as tpref
                    //foreach (var room in propertyDetails.Rooms)
                    //{
                    //    var referenceValues = Helper.GetReferenceValues(room.ThirdPartyReference);

                    //    // build the RoomRateElement here and put it into tpref2
                    //    RoomRate roomPriceDetails;

                    //    if (priceChanged)
                    //    {
                    //        roomPriceDetails = availabilityPricingResponse
                    //            .PriceVerification.HotelRooms
                    //            .First(r => r.RoomIndex == referenceValues.RoomIndex)
                    //            .RoomRate;
                    //    }
                    //    else
                    //    {
                    //        string[] roomRateInfo = referenceValues.RoomRateInfo.Split(',');

                    //        roomPriceDetails = new RoomRate
                    //        {
                    //            B2CRates = roomRateInfo[0].ToSafeBoolean(),
                    //            AgentMarkUp = roomRateInfo[1].ToSafeDecimal(),
                    //            RoomTax = roomRateInfo[2].ToSafeDecimal(),
                    //            RoomFare = roomRateInfo[3].ToSafeDecimal(),
                    //            Currency = roomRateInfo[4],
                    //            TotalFare = roomRateInfo[5].ToSafeDecimal()
                    //        };
                    //    }

                    //    priceDetails += $"{Helper.CleanRequest(_serializer.Serialize(roomPriceDetails).InnerXml)}|";
                    //}

                    //propertyDetails.TPRef1 = priceDetails.Chop();

                    var newLocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);

                    if (propertyDetails.LocalCost != newLocalCost)
                    {
                        propertyDetails.LocalCost = newLocalCost;
                    }

                    var cancellationList = new Cancellations();
                    foreach (var response in responses)
                    {
                        //var completePolicy =
                        //    string.IsNullOrEmpty(response.HotelResult
                        //        .SelectMany(x => x.Rooms)
                        //        .SelectMany(c => c.CancelPolicies)
                        //        .Select(c => c.Index)
                        //        .FirstOrDefault());

                        //var cost = !completePolicy
                        //    ? propertyDetails.Rooms
                        //        .First(x => x.ThirdPartyReference.Split(Helper.Separators, StringSplitOptions.None)[0] ==
                        //                    response.HotelResult.SelectMany(x => x.Rooms).First().BookingCode).LocalCost
                        //    : propertyDetails.Rooms.Where(x => );


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
                                    .OrderBy(x => x.FromDate)
                                    .First()
                                    .ToSafeDate()
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
            var reference = string.Empty;
            var url = /*_settings.BookingURL(propertyDetails)*/ "http://api.tbotechnology.in/TBOHolidays_HotelAPI/Book";
            var requests = new List<Request>();
            var requestsDetails = new List<BookingDetailRequest>();

            try
            {
                var auth = Helper.GetAuth(_settings.User(propertyDetails), _settings.Password(propertyDetails));

                foreach (var room in propertyDetails.Rooms)
                {
                    var bookRequest = await BuildBookRequestAsync(propertyDetails, room);
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

                if (requestsDetails.Any(rd => rd.BookResponse.Status.Code != 200 || rd.BookResponse.Status.Description != "Successful"))
                {
                    var detailUrl = "http://api.tbotechnology.in/TBOHolidays_HotelAPI/BookingDetail";

                    foreach (var requestsDetail in requestsDetails
                                 .Where(r => r.BookResponse.Status.Code != 200 || r.BookResponse.Status.Description != "Successful"))
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

                //if (bookResponse.BookingStatus == Models.Common.BookingType.Vouchered && bookResponse.Status.StatusCode == "01")
                //{
                //    propertyDetails.SourceSecondaryReference = $"{bookResponse.BookingId}-{bookResponse.TripId}";
                //    reference = bookResponse.ConfirmationNo;
                //}
                //else
                //{
                //    propertyDetails.Warnings.AddNew("Book Exception", "Failed to confirm booking");
                //    reference = "failed";
                //}
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
                reference = "failed";
            }
            finally
            {
                //propertyDetails.AddLog("Book", request);
            }

            return reference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var request = new Request();

            //try
            //{
            //    // check whether cancellation is allowed
            //    // only allow cancellations if current date is outside of cancellation period
            //    bool allowed = false;

            //    var bookingDetailRequest = new Envelope<HotelBookingDetailRequest>();
            //    bookingDetailRequest.Body.Content.BookingId = propertyDetails.SourceSecondaryReference.Split('-')[0].ToSafeInt();
            //    bookingDetailRequest.Header = BuildHeader("HotelBookingDetail", propertyDetails, _settings);

            //    var bookingDetailWebRequest = CreateRequest("HotelBookingDetail", _settings.GenericURL(propertyDetails));
            //    bookingDetailWebRequest.SetRequest(Helper.CleanRequest(_serializer.Serialize(bookingDetailRequest).OuterXml));
            //    await bookingDetailWebRequest.Send(_httpClient, _logger);

            //    var bookingDetailResponse = _serializer.DeSerialize<Envelope<HotelBookingDetailResponse>>(bookingDetailWebRequest.ResponseXML).Body.Content;

            //    // get cancellations from the reservation - proeprty specific cancellations
            //    var policies = bookingDetailResponse.BookingDetail.HotelCancelPolicies;
            //    if (bookingDetailResponse.Status.StatusCode == "01" && policies.PolicyFormat == PolicyFormat.Nodes)
            //    {
            //        foreach (var cancelPolicy in policies.CancelPolicies.OfType<CancelPolicy>().OrderBy(n => n.FromDate.ToSafeDate()))
            //        {
            //            if (DateTime.Now >= cancelPolicy.FromDate.ToSafeDate()
            //                && DateTime.Now <= cancelPolicy.ToDate.ToSafeDate())
            //            {
            //                allowed = cancelPolicy.CancellationCharge == 0;
            //            }
            //        }
            //    }

            //    if (allowed)
            //    {
            //        var cancelRequestObj = BuildCancelRequest(propertyDetails);
            //        cancelRequestObj.Header = BuildHeader("HotelCancel", propertyDetails, _settings);

            //        var cancelRequestXml = Helper.CleanRequest(_serializer.Serialize(cancelRequestObj).OuterXml);
            //        request = CreateRequest("HotelCancel", _settings.GenericURL(propertyDetails));
            //        request.SetRequest(cancelRequestXml);
            //        await request.Send(_httpClient, _logger);

            //        var response = _serializer.DeSerialize<Envelope<HotelCancelResponse>>(request.ResponseXML);
            //        var cancelResponse = response.Body.Content;

            //        if (cancelResponse.Status.StatusCode == "01" &&
            //            cancelResponse.RequestStatus == RequestStatus.Processed)
            //        {
            //            thirdPartyCancellationResponse.Success = true;
            //        }
            //        else
            //        {
            //            thirdPartyCancellationResponse.Success = false;
            //        }
            //    }
            //    else
            //    {
            //        propertyDetails.Warnings.AddNew(
            //            "Cancellation Exception",
            //            "Current date is outside of free cancellation period");

            //        thirdPartyCancellationResponse.Success = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
            //    thirdPartyCancellationResponse.Success = false;
            //}
            //finally
            //{
            //    propertyDetails.AddLog("Cancel", request);
            //}

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

        private async Task<HotelBookRequest> BuildBookRequestAsync(PropertyDetails propertyDetails, RoomDetails room)
        {
            var guestNationality = string.Empty;

            if (!string.IsNullOrEmpty(propertyDetails.ISONationalityCode))
            {
                guestNationality = await _support.TPNationalityLookupAsync(propertyDetails.Source, propertyDetails.ISONationalityCode);
            }

            if (string.IsNullOrEmpty(guestNationality))
            {
                guestNationality = _settings.LeadGuestNationality(propertyDetails);
            }

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
                                    ? GuestType.Child
                                    : GuestType.Adult
                            }).ToArray()
                    }
                },
                TotalFare = room.TotalCost,
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

        //private static Envelope<HotelCancelRequest> BuildCancelRequest(PropertyDetails propertyDetails)
        //{
        //    return newEnvelope<HotelCancelRequest>
        //    {
        //        Body = new Envelope<HotelCancelRequest>.SoapBody
        //        {
        //            Content = new HotelCancelRequest
        //            {
        //                BookingId = propertyDetails.SourceSecondaryReference.Split('-')[0].ToSafeInt(),
        //                RequestType = RequestType.HotelCancel,
        //                Remarks = "Cancel Booking"
        //            }
        //        }
        //    };
        //}

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

        private class BookingDetailRequest
        {
            public Request BookWebRequest { get; set; }
            public HotelBookRequest BookRequest { get; set; }
            public HotelBookResponse? BookResponse { get; set; }
            public Request BookDetailWebRequest { get; set; }
            public HotelBookingDetailResponse? BookingDetailResponse { get; set; }
        }
    } 
}