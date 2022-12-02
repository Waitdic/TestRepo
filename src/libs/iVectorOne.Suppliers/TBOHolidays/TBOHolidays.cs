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

                if (responses.All(r => r.Status.Code == "200" && r.Status.Description == "Successful"))
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

                    //cancellationList[0].StartDate.AddDays(2);
                    //cancellationList[1].StartDate.AddDays(1);

                    //cancellationList.AddNew(
                    //    cancellationList[0].StartDate.AddDays(-3),
                    //    cancellationList[0].StartDate.AddSeconds(-1),
                    //    cancellationList[0].Amount / 2);

                    //cancellationList.AddNew(
                    //    cancellationList[1].StartDate.AddDays(-2),
                    //    cancellationList[1].StartDate.AddSeconds(-1),
                    //    cancellationList[1].Amount / 2);

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
            string reference = string.Empty;
            //string url = _settings.GenericURL(propertyDetails);
            //var request = new Request();

            //try
            //{
            //    var bookRequest = await BuildBookRequestAsync(propertyDetails);
            //    bookRequest.Header = BuildHeader("HotelBook", propertyDetails, _settings);

            //    var bookRequestXml = Helper.CleanRequest(_serializer.Serialize(bookRequest).OuterXml);
            //    request = CreateRequest("HotelBook", url);
            //    request.SetRequest(bookRequestXml);
            //    await request.Send(_httpClient, _logger);

            //    var response = _serializer.DeSerialize<Envelope<HotelBookResponse>>(request.ResponseXML);
            //    var bookResponse = response.Body.Content;

            //    if (bookResponse.BookingStatus == Models.Common.BookingStatus.Vouchered && bookResponse.Status.StatusCode == "01")
            //    {
            //        propertyDetails.SourceSecondaryReference = $"{bookResponse.BookingId}-{bookResponse.TripId}";
            //        reference = bookResponse.ConfirmationNo;
            //    }
            //    else
            //    {
            //        propertyDetails.Warnings.AddNew("Book Exception", "Failed to confirm booking");
            //        reference = "failed";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    propertyDetails.Warnings.AddNew("Book Exception", ex.ToString());
            //    reference = "failed";
            //}
            //finally
            //{
            //    propertyDetails.AddLog("Book", request);
            //}

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

            //var referenceValues = Helper.GetReferenceValues(reference);
            //return new Envelope<AvailabilityAndPricingRequest>
            //{
            //    Body = new Envelope<AvailabilityAndPricingRequest>.SoapBody
            //    {
            //        Content = new AvailabilityAndPricingRequest
            //        {
            //            ResultIndex = referenceValues.ResultIndex,
            //            HotelCode = propertyDetails.TPKey,
            //            SessionId = referenceValues.SessionId,
            //            OptionsForBooking = new OptionsForBooking
            //            {
            //                FixedFormat = false,
            //                RoomCombination = new[]
            //                {
            //                    new RoomCombination
            //                    {
            //                        RoomIndex = propertyDetails.Rooms
            //                            .Select(r => Helper.GetReferenceValues(r.ThirdPartyReference).RoomIndex)
            //                            .ToArray()
            //                    }
            //                }
            //            }
            //        }
            //    }
            //};
        }

        //private async Task<Envelope<HotelBookRequest>> BuildBookRequestAsync(PropertyDetails propertyDetails)
        //{
        //    var referenceValues = Helper.GetReferenceValues(propertyDetails.Rooms[0].ThirdPartyReference);

        //    int resultIndex = referenceValues.ResultIndex;
        //    string sessionId = referenceValues.SessionId;
        //    string clientCode = _settings.ClientCode(propertyDetails);

        //    string guestNationality = string.Empty;

        //    if (!string.IsNullOrEmpty(propertyDetails.ISONationalityCode))
        //    {
        //        guestNationality = await _support.TPNationalityLookupAsync(propertyDetails.Source, propertyDetails.ISONationalityCode);
        //    }

        //    if (string.IsNullOrEmpty(guestNationality))
        //    {
        //        guestNationality = _settings.LeadGuestNationality(propertyDetails);
        //    }

        //    int passengerIndex = 1;
        //    int mockRoomIndex = 1;
        //    var guests = new List<Guest>();
        //    foreach (var room in propertyDetails.Rooms)
        //    {
        //        foreach (var passenger in room.Passengers)
        //        {
        //            string title = passenger.Title switch
        //            {
        //                "Mr" or "Miss" or "Mrs" or "Ms" => passenger.Title,
        //                _ => "Mr",
        //            };
        //            guests.Add(new Guest
        //            {
        //                GuestInRoom = mockRoomIndex,
        //                Title = title,
        //                LeadGuest = passengerIndex == 1,
        //                FirstName = passenger.FirstName.Length == 1 ? "TBA" : passenger.FirstName,
        //                LastName = passenger.LastName.Length == 1 ? "TBA" : passenger.LastName,

        //                GuestType = passenger.PassengerType is PassengerType.Child or PassengerType.Infant
        //                    ? GuestType.Child
        //                    : GuestType.Adult,

        //                Age = passenger.PassengerType is PassengerType.Child or PassengerType.Infant
        //                    ? passenger.Age
        //                    : null
        //            });

        //            passengerIndex++;
        //        }

        //        mockRoomIndex++;
        //    }

        //    string[] roomRateRefs = propertyDetails.TPRef1.Split('|');
        //    var hotelRooms = new List<HotelRoom>();
        //    int roomPriceIndex = 0;
        //    foreach (var room in propertyDetails.Rooms)
        //    {
        //        var roomReferenceValues = Helper.GetReferenceValues(room.ThirdPartyReference);
        //        string supplementInformation = roomReferenceValues.SupplementInformation.Chop();

        //        var supplements = new List<SuppInfo>();
        //        if (!string.IsNullOrEmpty(supplementInformation))
        //        {
        //            supplements.AddRange(supplementInformation.Split('|').Select(x => new SuppInfo
        //            {
        //                SuppChargeType = SuppChargeType.AtProperty,
        //                SuppIsSelected = false,
        //                SuppID = x.Split(',')[0].ToSafeInt(),
        //                Price = x.Split(',')[1].ToSafeInt()
        //            }));
        //        }

        //        hotelRooms.Add(new HotelRoom
        //        {
        //            RoomIndex = Helper.GetReferenceValues(room.ThirdPartyReference).RoomIndex,
        //            Name = room.RoomType,
        //            RoomTypeCode = roomReferenceValues.RoomTypeCode,
        //            RatePlanCode = roomReferenceValues.RatePlanCode,
        //            RoomRate = _serializer.DeSerialize<RoomRate>(roomRateRefs[roomPriceIndex]),
        //            Supplements = new Supplements { SuppInfo = supplements.ToArray() }
        //        });

        //        roomPriceIndex++;
        //    }

        //    var specialRequests = new List<SpecialRequests>();
        //    if (propertyDetails.BookingComments.Count > 0)
        //    {
        //        int commentIndex = 1;
        //        foreach (var bookingComment in propertyDetails.BookingComments)
        //        {
        //            specialRequests.Add(new SpecialRequests
        //            {
        //                RequestId = commentIndex,
        //                Remarks = bookingComment.Text,
        //                RequestType = "BookingComment"
        //            });
        //            commentIndex++;
        //        }
        //    }

        //    var request = new HotelBookRequest
        //    {
        //        ClientReferenceNumber = $"{DateTime.Now:ddMMyyhhmmssfff}#{clientCode}",
        //        GuestNationality = guestNationality,
        //        Guests = guests.ToArray(),
        //        HotelRooms = hotelRooms.ToArray(),
        //        AddressInfo = new AddressInfo
        //        {
        //            AddressLine1 = propertyDetails.LeadGuestAddress1,
        //            AddressLine2 = propertyDetails.LeadGuestAddress2,
        //            PhoneNo = propertyDetails.LeadGuestPhone,
        //            Email = propertyDetails.LeadGuestEmail,
        //            City = propertyDetails.LeadGuestTownCity,
        //            State = propertyDetails.LeadGuestCounty,
        //            Country = await _support.TPCountryCodeLookupAsync(propertyDetails.Source, propertyDetails.LeadGuestCountryCode, propertyDetails.AccountID),
        //            ZipCode = propertyDetails.LeadGuestPostcode,
        //        },
        //        PaymentInfo = new PaymentInfo
        //        {
        //            VoucherBooking = true,
        //            PaymentModeType =(PaymentModeType) Enum.Parse(
        //                typeof(PaymentModeType),
        //                _settings.PaymentModeType(propertyDetails),
        //                true)
        //        },
        //        SessionId = sessionId,
        //        NoOfRooms = propertyDetails.Rooms.Count,
        //        ResultIndex = resultIndex,
        //        HotelCode = propertyDetails.TPKey,
        //        HotelName = propertyDetails.PropertyName,
        //        SpecialRequests = specialRequests.ToArray()
        //    };

        //    return new Envelope<HotelBookRequest>
        //    {
        //        Body = new Envelope<HotelBookRequest>.SoapBody { Content = request }
        //    };
        //}

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
    } 
}