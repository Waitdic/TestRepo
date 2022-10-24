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
            // no actual prebook, do an availability and price check, grab cancellation policies
            bool success = false;
            bool priceChanged = false;
            bool cancellationPoliciesAvailable = false;
            string url = _settings.GenericURL(propertyDetails);
            var request = new Request();
            var policyRequest = new Request();

            // availability and pricing
            try
            {
                var availabilityRequest = BuildAvailabilityRequest(propertyDetails);
                availabilityRequest.Header = BuildHeader("AvailabilityAndPricing", propertyDetails, _settings);

                var requestXml = Helper.CleanRequest(_serializer.Serialize(availabilityRequest).OuterXml);
                request = CreateRequest("AvailabilityAndPricing", url);
                request.SetRequest(requestXml);
                await request.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<Envelope<AvailabilityAndPricingResponse>>(request.ResponseXML);
                var availabilityPricingResponse = response.Body.Content;

                // cancellation policies available
                cancellationPoliciesAvailable = availabilityPricingResponse.CancellationPoliciesAvailable;
                string availabilityStatusCode = availabilityPricingResponse.Status.StatusCode;
                var priceVerificationStatus = availabilityPricingResponse.PriceVerification.Status;

                // price check
                if (availabilityStatusCode == "01"
                    && (priceVerificationStatus == StatusEnum.Successful ||
                        priceVerificationStatus == StatusEnum.NotAvailable))
                {
                    if (availabilityPricingResponse.PriceVerification.PriceChanged)
                    {
                        if (availabilityPricingResponse.PriceVerification.AvailableOnNewPrice)
                        {
                            // do the price change
                            foreach (var room in propertyDetails.Rooms)
                            {
                                int roomIndex = Helper.GetReferenceValues(room.ThirdPartyReference).RoomIndex;
                                decimal newPrice = availabilityPricingResponse.PriceVerification.HotelRooms
                                    .Where(r => r.RoomIndex == roomIndex)
                                    .Select(rate => rate.RoomRate.TotalFare)
                                    .First();

                                if (newPrice != room.LocalCost)
                                {
                                    room.LocalCost = newPrice;
                                    room.GrossCost = newPrice;
                                    priceChanged = true;
                                }
                            }

                            success = true;
                        }
                        else
                        {
                            propertyDetails.Warnings.AddNew("Prebook Exception", "New Price not Available");
                            success = false;
                        }
                    }
                    else
                    {
                        // successful and no price changed
                        success = true;
                    }

                    if (success)
                    {
                        string priceDetails = string.Empty;

                        // grab the price details and store as tpref
                        foreach (var room in propertyDetails.Rooms)
                        {
                            var referenceValues = Helper.GetReferenceValues(room.ThirdPartyReference);

                            // build the RoomRateElement here and put it into tpref2
                            RoomRate roomPriceDetails;

                            if (priceChanged)
                            {
                                roomPriceDetails = availabilityPricingResponse
                                    .PriceVerification.HotelRooms
                                    .First(r => r.RoomIndex == referenceValues.RoomIndex)
                                    .RoomRate;
                            }
                            else
                            {
                                string[] roomRateInfo = referenceValues.RoomRateInfo.Split(',');

                                roomPriceDetails = new RoomRate
                                {
                                    B2CRates = roomRateInfo[0].ToSafeBoolean(),
                                    AgentMarkUp = roomRateInfo[1].ToSafeDecimal(),
                                    RoomTax = roomRateInfo[2].ToSafeDecimal(),
                                    RoomFare = roomRateInfo[3].ToSafeDecimal(),
                                    Currency = roomRateInfo[4],
                                    TotalFare = roomRateInfo[5].ToSafeDecimal()
                                };
                            }

                            priceDetails += $"{Helper.CleanRequest(_serializer.Serialize(roomPriceDetails).InnerXml)}|";
                        }

                        propertyDetails.TPRef1 = priceDetails.Chop();
                    }
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
                propertyDetails.AddLog("Availability Check", request);
            }

            if (success && cancellationPoliciesAvailable)
            {
                try
                {
                    success = false;

                    var policyRequestObj = BuildPolicyRequest(propertyDetails);
                    policyRequestObj.Header = BuildHeader("HotelCancellationPolicy", propertyDetails, _settings);

                    var policyRequestXml = Helper.CleanRequest(_serializer.Serialize(policyRequestObj).OuterXml);
                    policyRequest = CreateRequest("HotelCancellationPolicies", url);
                    policyRequest.SetRequest(policyRequestXml);
                    await policyRequest.Send(_httpClient, _logger);

                    var response = _serializer.DeSerialize<Envelope<HotelCancellationPolicyResponse>>(policyRequest.ResponseXML);
                    var policyResponse = response.Body.Content;

                    bool completePolicy =
                        string.IsNullOrEmpty(policyResponse.CancelPolicies.OfType<CancelPolicy>().Select(c => c.RoomIndex).FirstOrDefault());
                    decimal newLocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);

                    if (propertyDetails.LocalCost != newLocalCost)
                    {
                        propertyDetails.LocalCost = newLocalCost;
                    }

                    if (policyResponse.Status.StatusCode == "01")
                    {
                        // TBOHolidays can return cancellation policies in 2 different formats depending on whats available to them
                        // node format policies - individual rooms
                        if (!completePolicy)
                        {
                            foreach (var room in propertyDetails.Rooms)
                            {
                                int roomIndex = Helper.GetReferenceValues(room.ThirdPartyReference).RoomIndex;
                                foreach (var policyNode in policyResponse.CancelPolicies.OfType<CancelPolicy>().Where(r =>
                                             r.RoomIndex.ToSafeInt() == roomIndex))
                                {
                                    decimal amount = 0;
                                    switch (policyNode.ChargeType)
                                    {
                                        case ChargeType.Fixed:
                                            amount = policyNode.CancellationCharge;
                                            break;
                                        case ChargeType.Percentage:
                                            amount = (policyNode.CancellationCharge / 100) * room.LocalCost;
                                            break;
                                    }

                                    propertyDetails.Cancellations.AddNew(
                                        policyNode.FromDate.ToSafeDate(),
                                        policyNode.ToDate.ToSafeDate(),
                                        amount);
                                }
                            }
                        }
                        else
                        {
                            foreach (var policyNode in policyResponse.CancelPolicies.OfType<CancelPolicy>())
                            {
                                decimal amount = 0;
                                switch (policyNode.ChargeType)
                                {
                                    case ChargeType.Fixed:
                                        amount = policyNode.CancellationCharge;
                                        break;
                                    case ChargeType.Percentage:
                                        amount = (policyNode.CancellationCharge / 100) * propertyDetails.LocalCost;
                                        break;
                                }

                                propertyDetails.Cancellations.AddNew(
                                    policyNode.FromDate.ToSafeDate(),
                                    policyNode.ToDate.ToSafeDate(),
                                    amount);
                            }
                        }

                        propertyDetails.Cancellations.Solidify(SolidifyType.Sum);
                        success = true;

                        foreach (string hotelNorm in policyResponse.HotelNorms)
                        {
                            if (!string.IsNullOrEmpty(hotelNorm))
                            {
                                propertyDetails.Errata.AddNew("Important Information", hotelNorm);
                            }
                        }

                        foreach (var hotelPolicy in policyResponse.CancelPolicies.OfType<DefaultPolicy>())
                        {
                            if (!string.IsNullOrEmpty(hotelPolicy.Value))
                            {
                                propertyDetails.Errata.AddNew("Important Information", hotelPolicy.Value);
                            }
                        }
                    }
                    else
                    {
                        propertyDetails.Warnings.AddNew("Cancellation Policies Exception",
                            "Failed to get cancellation policies");
                        success = false;
                    }
                }
                catch (Exception ex)
                {
                    propertyDetails.Warnings.AddNew("Cancellation Policies Exception", ex.ToString());
                }
                finally
                {
                    propertyDetails.AddLog("Cancellation Policies", policyRequest);
                }
            }

            return success;
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string reference;
            string url = _settings.GenericURL(propertyDetails);
            var request = new Request();

            try
            {
                var bookRequest = await BuildBookRequestAsync(propertyDetails);
                bookRequest.Header = BuildHeader("HotelBook", propertyDetails, _settings);

                var bookRequestXml = Helper.CleanRequest(_serializer.Serialize(bookRequest).OuterXml);
                request = CreateRequest("HotelBook", url);
                request.SetRequest(bookRequestXml);
                await request.Send(_httpClient, _logger);

                var response = _serializer.DeSerialize<Envelope<HotelBookResponse>>(request.ResponseXML);
                var bookResponse = response.Body.Content;

                if (bookResponse.BookingStatus == Models.Common.BookingStatus.Vouchered && bookResponse.Status.StatusCode == "01")
                {
                    propertyDetails.SourceSecondaryReference = $"{bookResponse.BookingId}-{bookResponse.TripId}";
                    reference = bookResponse.ConfirmationNo;
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
                propertyDetails.AddLog("Book", request);
            }

            return reference;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var request = new Request();

            try
            {
                // check whether cancellation is allowed
                // only allow cancellations if current date is outside of cancellation period
                bool allowed = false;

                var bookingDetailRequest = new Envelope<HotelBookingDetailRequest>();
                bookingDetailRequest.Body.Content.BookingId = propertyDetails.SourceSecondaryReference.Split('-')[0].ToSafeInt();
                bookingDetailRequest.Header = BuildHeader("HotelBookingDetail", propertyDetails, _settings);

                var bookingDetailWebRequest = CreateRequest("HotelBookingDetail", _settings.GenericURL(propertyDetails));
                bookingDetailWebRequest.SetRequest(Helper.CleanRequest(_serializer.Serialize(bookingDetailRequest).OuterXml));
                await bookingDetailWebRequest.Send(_httpClient, _logger);

                var bookingDetailResponse = _serializer.DeSerialize<Envelope<HotelBookingDetailResponse>>(bookingDetailWebRequest.ResponseXML).Body.Content;

                // get cancellations from the reservation - proeprty specific cancellations
                var policies = bookingDetailResponse.BookingDetail.HotelCancelPolicies;
                if (bookingDetailResponse.Status.StatusCode == "01" && policies.PolicyFormat == PolicyFormat.Nodes)
                {
                    foreach (var cancelPolicy in policies.CancelPolicies.OfType<CancelPolicy>().OrderBy(n => n.FromDate.ToSafeDate()))
                    {
                        if (DateTime.Now >= cancelPolicy.FromDate.ToSafeDate()
                            && DateTime.Now <= cancelPolicy.ToDate.ToSafeDate())
                        {
                            allowed = cancelPolicy.CancellationCharge == 0;
                        }
                    }
                }

                if (allowed)
                {
                    var cancelRequestObj = BuildCancelRequest(propertyDetails);
                    cancelRequestObj.Header = BuildHeader("HotelCancel", propertyDetails, _settings);

                    var cancelRequestXml = Helper.CleanRequest(_serializer.Serialize(cancelRequestObj).OuterXml);
                    request = CreateRequest("HotelCancel", _settings.GenericURL(propertyDetails));
                    request.SetRequest(cancelRequestXml);
                    await request.Send(_httpClient, _logger);

                    var response = _serializer.DeSerialize<Envelope<HotelCancelResponse>>(request.ResponseXML);
                    var cancelResponse = response.Body.Content;

                    if (cancelResponse.Status.StatusCode == "01" &&
                        cancelResponse.RequestStatus == RequestStatus.Processed)
                    {
                        thirdPartyCancellationResponse.Success = true;
                    }
                    else
                    {
                        thirdPartyCancellationResponse.Success = false;
                    }
                }
                else
                {
                    propertyDetails.Warnings.AddNew(
                        "Cancellation Exception",
                        "Current date is outside of free cancellation period");

                    thirdPartyCancellationResponse.Success = false;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Cancellation Exception", ex.ToString());
                thirdPartyCancellationResponse.Success = false;
            }
            finally
            {
                propertyDetails.AddLog("Cancel", request);
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

        private static Envelope<AvailabilityAndPricingRequest> BuildAvailabilityRequest(PropertyDetails propertyDetails)
        {
            string reference = propertyDetails.Rooms[0].ThirdPartyReference;
            var referenceValues = Helper.GetReferenceValues(reference);

            return new Envelope<AvailabilityAndPricingRequest>
            {
                Body = new Envelope<AvailabilityAndPricingRequest>.SoapBody
                {
                    Content = new AvailabilityAndPricingRequest
                    {
                        ResultIndex = referenceValues.ResultIndex,
                        HotelCode = propertyDetails.TPKey,
                        SessionId = referenceValues.SessionId,
                        OptionsForBooking = new OptionsForBooking
                        {
                            FixedFormat = false,
                            RoomCombination = new[]
                            {
                                new RoomCombination
                                {
                                    RoomIndex = propertyDetails.Rooms
                                        .Select(r => Helper.GetReferenceValues(r.ThirdPartyReference).RoomIndex)
                                        .ToArray()
                                }
                            }
                        }
                    }
                }
            };
        }

        private static Envelope<HotelCancellationPolicyRequest> BuildPolicyRequest(PropertyDetails propertyDetails)
        {
            string reference = propertyDetails.Rooms[0].ThirdPartyReference;
            var referenceValues = Helper.GetReferenceValues(reference);

            return new Envelope<HotelCancellationPolicyRequest>
            {
                Body = new Envelope<HotelCancellationPolicyRequest>.SoapBody
                {
                    Content = new HotelCancellationPolicyRequest
                    {
                        ResultIndex = referenceValues.ResultIndex,
                        HotelCode = propertyDetails.TPRef1,
                        SessionId = referenceValues.SessionId,
                        OptionsForBooking = new OptionsForBooking
                        {
                            FixedFormat = false,
                            RoomCombination = new[]
                            {
                                new RoomCombination
                                {
                                    RoomIndex = propertyDetails.Rooms
                                        .Select(r => Helper.GetReferenceValues(r.ThirdPartyReference).RoomIndex)
                                        .ToArray()
                                }
                            }
                        }
                    }
                }
            };
        }

        private async Task<Envelope<HotelBookRequest>> BuildBookRequestAsync(PropertyDetails propertyDetails)
        {
            var referenceValues = Helper.GetReferenceValues(propertyDetails.Rooms[0].ThirdPartyReference);

            int resultIndex = referenceValues.ResultIndex;
            string sessionId = referenceValues.SessionId;
            string clientCode = _settings.ClientCode(propertyDetails);

            string guestNationality = string.Empty;

            if (!string.IsNullOrEmpty(propertyDetails.ISONationalityCode))
            {
                guestNationality = await _support.TPNationalityLookupAsync(propertyDetails.Source, propertyDetails.ISONationalityCode);
            }

            if (string.IsNullOrEmpty(guestNationality))
            {
                guestNationality = _settings.LeadGuestNationality(propertyDetails);
            }

            int passengerIndex = 1;
            int mockRoomIndex = 1;
            var guests = new List<Guest>();
            foreach (var room in propertyDetails.Rooms)
            {
                foreach (var passenger in room.Passengers)
                {
                    string title = passenger.Title switch
                    {
                        "Mr" or "Miss" or "Mrs" or "Ms" => passenger.Title,
                        _ => "Mr",
                    };
                    guests.Add(new Guest
                    {
                        GuestInRoom = mockRoomIndex,
                        Title = title,
                        LeadGuest = passengerIndex == 1,
                        FirstName = passenger.FirstName.Length == 1 ? "TBA" : passenger.FirstName,
                        LastName = passenger.LastName.Length == 1 ? "TBA" : passenger.LastName,

                        GuestType = passenger.PassengerType is PassengerType.Child or PassengerType.Infant
                            ? GuestType.Child
                            : GuestType.Adult,

                        Age = passenger.PassengerType is PassengerType.Child or PassengerType.Infant
                            ? passenger.Age
                            : null
                    });

                    passengerIndex++;
                }

                mockRoomIndex++;
            }

            string[] roomRateRefs = propertyDetails.TPRef1.Split('|');
            var hotelRooms = new List<HotelRoom>();
            int roomPriceIndex = 0;
            foreach (var room in propertyDetails.Rooms)
            {
                var roomReferenceValues = Helper.GetReferenceValues(room.ThirdPartyReference);
                string supplementInformation = roomReferenceValues.SupplementInformation.Chop();

                var supplements = new List<SuppInfo>();
                if (!string.IsNullOrEmpty(supplementInformation))
                {
                    supplements.AddRange(supplementInformation.Split('|').Select(x => new SuppInfo
                    {
                        SuppChargeType = SuppChargeType.AtProperty,
                        SuppIsSelected = false,
                        SuppID = x.Split(',')[0].ToSafeInt(),
                        Price = x.Split(',')[1].ToSafeInt()
                    }));
                }

                hotelRooms.Add(new HotelRoom
                {
                    RoomIndex = Helper.GetReferenceValues(room.ThirdPartyReference).RoomIndex,
                    RoomTypeName = room.RoomType,
                    RoomTypeCode = roomReferenceValues.RoomTypeCode,
                    RatePlanCode = roomReferenceValues.RatePlanCode,
                    RoomRate = _serializer.DeSerialize<RoomRate>(roomRateRefs[roomPriceIndex]),
                    Supplements = new Supplements { SuppInfo = supplements.ToArray() }
                });

                roomPriceIndex++;
            }

            var specialRequests = new List<SpecialRequests>();
            if (propertyDetails.BookingComments.Count > 0)
            {
                int commentIndex = 1;
                foreach (var bookingComment in propertyDetails.BookingComments)
                {
                    specialRequests.Add(new SpecialRequests
                    {
                        RequestId = commentIndex,
                        Remarks = bookingComment.Text,
                        RequestType = "BookingComment"
                    });
                    commentIndex++;
                }
            }

            var request = new HotelBookRequest
            {
                ClientReferenceNumber = $"{DateTime.Now:ddMMyyhhmmssfff}#{clientCode}",
                GuestNationality = guestNationality,
                Guests = guests.ToArray(),
                HotelRooms = hotelRooms.ToArray(),
                AddressInfo = new AddressInfo
                {
                    AddressLine1 = propertyDetails.LeadGuestAddress1,
                    AddressLine2 = propertyDetails.LeadGuestAddress2,
                    PhoneNo = propertyDetails.LeadGuestPhone,
                    Email = propertyDetails.LeadGuestEmail,
                    City = propertyDetails.LeadGuestTownCity,
                    State = propertyDetails.LeadGuestCounty,
                    Country = await _support.TPCountryCodeLookupAsync(propertyDetails.Source, propertyDetails.LeadGuestCountryCode, propertyDetails.AccountID),
                    ZipCode = propertyDetails.LeadGuestPostcode,
                },
                PaymentInfo = new PaymentInfo
                {
                    VoucherBooking = true,
                    PaymentModeType =(PaymentModeType) Enum.Parse(
                        typeof(PaymentModeType),
                        _settings.PaymentModeType(propertyDetails),
                        true)
                },
                SessionId = sessionId,
                NoOfRooms = propertyDetails.Rooms.Count,
                ResultIndex = resultIndex,
                HotelCode = propertyDetails.TPKey,
                HotelName = propertyDetails.PropertyName,
                SpecialRequests = specialRequests.ToArray()
            };

            return new Envelope<HotelBookRequest>
            {
                Body = new Envelope<HotelBookRequest>.SoapBody { Content = request }
            };
        }

        private static Envelope<HotelCancelRequest> BuildCancelRequest(PropertyDetails propertyDetails)
        {
            return new Envelope<HotelCancelRequest>
            {
                Body = new Envelope<HotelCancelRequest>.SoapBody
                {
                    Content = new HotelCancelRequest
                    {
                        BookingId = propertyDetails.SourceSecondaryReference.Split('-')[0].ToSafeInt(),
                        RequestType = RequestType.HotelCancel,
                        Remarks = "Cancel Booking"
                    }
                }
            };
        }

        public static SoapHeader BuildHeader(
            string type,
            IThirdPartyAttributeSearch searchDetails,
            ITBOHolidaysSettings settings)
        {
            return new SoapHeader
            {
                Credentials = new Credentials
                {
                    UserName = settings.User(searchDetails),
                    Password = settings.Password(searchDetails),
                },
                Action = $"http://TekTravel/HotelBookingApi/{type}",
                To = settings.GenericURL(searchDetails)
            };
        }

        private static Request CreateRequest(string type, string url)
        {
            return new Request
            {
                Source = ThirdParties.TBOHOLIDAYS,
                LogFileName = type,
                UseGZip = false,
                EndPoint = url,
                CreateLog = true,
                Method = RequestMethod.POST,
                ContentType = ContentTypes.Application_SOAP_XML
            };
        }
    } 
}