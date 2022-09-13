namespace iVectorOne.Suppliers.Hotelston
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
    using iVector.Search.Property;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.Hotelston.Models;
    using iVectorOne.Suppliers.Hotelston.Models.Common;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using RoomDetails = iVectorOne.Models.Property.Booking.RoomDetails;

    public class Hotelston : IThirdParty, ISingleSource
    {
        private readonly IHotelstonSettings _settings;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<Hotelston> _logger;

        public Hotelston(IHotelstonSettings settings, ISerializer serializer, HttpClient httpClient, ILogger<Hotelston> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        public string Source => ThirdParties.HOTELSTON;

        public bool SupportsRemarks => false;

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
            bool prebookSuccess = await CheckAvailabilityRequestAsync(propertyDetails);

            if (prebookSuccess)
            {
                prebookSuccess = await BookingTermsRequestAsync(propertyDetails);
            }

            return prebookSuccess;
        }

        private async Task<bool> CheckAvailabilityRequestAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            bool success;

            try
            {
                var envelope = HotelstonHelper.CreateEnvelope<CheckAvailabilityRequest>(_settings, propertyDetails);
                var request = envelope.Body.Content;

                request.Criteria.CheckIn = propertyDetails.ArrivalDate.ToString(HotelstonHelper.DateFormatString);
                request.Criteria.CheckOut = propertyDetails.DepartureDate.ToString(HotelstonHelper.DateFormatString);
                request.Criteria.HotelId = propertyDetails.TPKey;

                foreach (var roomDetail in propertyDetails.Rooms)
                {
                    string[] roomTypeCodeItems = roomDetail.RoomTypeCode.Split('|');

                    var room = new Room(roomDetail);
                    request.Criteria.Rooms.Add(new Models.Common.Room
                    {
                        Adults = room.Adults,
                        Children = room.Children,
                        ChildAges = room.ChildAges.Where(n => n <= 14).ToArray(),
                        RoomId = roomTypeCodeItems[0],
                        RoomTypeId = roomTypeCodeItems[1],
                        BoardTypeId = roomDetail.ThirdPartyReference.Split('|')[2]
                    });
                }

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.HOTELSTON,
                    LogFileName = "checkAvailability",
                    CreateLog = true,
                    ExtraInfo = propertyDetails,
                    SoapAction = "urn:checkAvailability",
                    UseGZip = true,
                    SOAP = true
                };
                webRequest.SetRequest(HotelstonHelper.Serialize(envelope, envelope.Xmlns, envelope.AttributeOverrides));
                await webRequest.Send(_httpClient, _logger);

                var response = HotelstonHelper.DeSerialize<CheckAvailabilityResponse>(webRequest.ResponseXML, _serializer);

                success = response.Success;

                if (success)
                {
                    success = CheckPrebookSuccess(response, propertyDetails);
                }

                if (success)
                {
                    success = ProcessCancellationInformation(response, propertyDetails);
                }
            }
            catch (Exception ex)
            {
                success = false;
                propertyDetails.Warnings.AddNew("Hotelston Check Availability Exception", ex.Message);
            }
            finally
            {
                propertyDetails.AddLog("Check Availability", webRequest);
            }

            return success;
        }

        private static bool ProcessCancellationInformation(CheckAvailabilityResponse response, PropertyDetails propertyDetails)
        {
            var policyEndDate = new DateTime(2099, 1, 1);
            bool success = false;

            foreach (var room in response.Hotel.Rooms)
            {
                decimal roomPrice = room.Price;
                var arrivalDate = propertyDetails.ArrivalDate;
                bool nonRefundable = room.BookingTerms.NonRefundable;

                DateTime policyStartDate;
                if (nonRefundable)
                {
                    success = true;
                    policyStartDate = DateTime.Now;
                    propertyDetails.Cancellations.AddNew(policyStartDate, policyEndDate, roomPrice);
                }
                else
                {
                    foreach (var cancellationRule in room.BookingTerms.CancellationPolicies.SelectMany(cp => cp.CancellationRules))
                    {
                        success = true;
                        string timeUnit = cancellationRule.CancellationDeadline.TimeUnit.ToUpper();

                        policyStartDate = timeUnit.Equals("FULL_DURATION")
                            ? DateTime.Today
                            : GetPolicyStartDate(arrivalDate, cancellationRule);

                        decimal feeAmount;
                        string penaltyUnit = cancellationRule.CancellationPenalty.PenaltyUnit;
                        decimal penaltyAmount = cancellationRule.CancellationPenalty.Amount;

                        switch (penaltyUnit)
                        {
                            case "NUMBER_OF_NIGHTS":
                            {
                                feeAmount = roomPrice * penaltyAmount;
                                break;
                            }
                            case "PERCENTAGE":
                            {
                                feeAmount = roomPrice * (penaltyAmount / 100m);
                                break;
                            }
                            case "MONEY_AMOUNT":
                            {
                                feeAmount = roomPrice;
                                break;
                            }
                            case "PERCENTAGE_OF_FIRST_NIGHT":
                            {
                                feeAmount = propertyDetails.Duration / roomPrice;
                                feeAmount *= (penaltyAmount / 100m);
                                break;
                            }
                            default:
                            {
                                return false;
                            }
                        }

                        if (feeAmount <= 0m | policyStartDate == default)
                        {
                            return false;
                        }

                        propertyDetails.Cancellations.AddNew(policyStartDate, policyEndDate, feeAmount);
                    }
                }
            }

            propertyDetails.Cancellations.Solidify(SolidifyType.Sum);
            return success;
        }

        private static DateTime GetPolicyStartDate(DateTime arrivalDate, CancellationRule cancellationRule)
        {
            var policyStartDate = default(DateTime);
            string effectMoment = cancellationRule.CancellationDeadline.EffectMoment;
            string timeUnit = cancellationRule.CancellationDeadline.TimeUnit.ToUpper();
            int amountTime = cancellationRule.CancellationDeadline.Amount;
            int effectMomentFactor = default;

            switch (effectMoment)
            {
                case "BEFORE_ARRIVAL":
                {
                    effectMomentFactor = -1;
                    policyStartDate = arrivalDate;
                    break;
                }
                case "AFTER_BOOKING":
                {
                    effectMomentFactor = 1;
                    policyStartDate = DateTime.Today;
                    break;
                }
            }

            switch (timeUnit)
            {
                case "YEAR":
                {
                    policyStartDate = policyStartDate.AddYears(amountTime * effectMomentFactor);
                    break;
                }
                case "MONTH":
                {
                    policyStartDate = policyStartDate.AddMonths(amountTime * effectMomentFactor);
                    break;
                }
                case "WEEK":
                {
                    int iWeeks = amountTime * 7;
                    policyStartDate = policyStartDate.AddDays(iWeeks * effectMomentFactor);
                    break;
                }
                case "DAY":
                {
                    policyStartDate = policyStartDate.AddDays(amountTime * effectMomentFactor);
                    break;
                }
                case "HOUR":
                {
                    policyStartDate = policyStartDate.AddHours(amountTime * effectMomentFactor);
                    break;
                }
                case "SECOND":
                {
                    policyStartDate = policyStartDate.AddSeconds(amountTime * effectMomentFactor);
                    break;
                }
            }

            return policyStartDate;
        }

        private static bool CheckPrebookSuccess(CheckAvailabilityResponse response, PropertyDetails propertyDetails)
        {
            bool prebookSuccess;
            string errorType = string.Empty;
            var hotel = response.Hotel;
            string searchHotelId = propertyDetails.TPKey;
            string preBookHotelId = hotel.Id;

            if (preBookHotelId != searchHotelId)
            {
                prebookSuccess = false;
                errorType = "Hotel ID Does Not Match";
            }
            else
            {
                prebookSuccess = true;
            }

            if (prebookSuccess)
            {
                int count = 0;
                foreach (var room in hotel.Rooms)
                {
                    var propertyRoom = propertyDetails.Rooms[count];
                    string[] roomTypeCodeItems = propertyRoom.RoomTypeCode.Split('|');

                    string searchRoomId = roomTypeCodeItems[0];
                    string searchRoomTypeId = roomTypeCodeItems[1];
                    string searchBoardTypeId = propertyRoom.ThirdPartyReference.Split('|')[2];
                    // use LocalCost because GrossCost is empty
                    decimal searchRoomCost = Math.Round(propertyRoom.LocalCost.ToSafeDecimal(), 2, MidpointRounding.AwayFromZero);
                    string preBookRoomId = room.Id;

                    if (prebookSuccess && preBookRoomId != searchRoomId)
                    {
                        prebookSuccess = false;
                        errorType = "Room ID Does Not Match";
                        break;
                    }

                    string preBookBoardTypeId = room.BoardType.Id;
                    if (prebookSuccess && preBookBoardTypeId != searchBoardTypeId)
                    {
                        prebookSuccess = false;
                        errorType = "BoardType ID Does Not Match";
                        break;
                    }

                    string preBookRoomTypeId = room.RoomType.Id;
                    if (prebookSuccess && preBookRoomTypeId != searchRoomTypeId)
                    {
                        prebookSuccess = false;
                        errorType = "RoomType ID Does Not Match";
                        break;
                    }

                    decimal preBookRoomCost = room.Price;
                    if (prebookSuccess && preBookRoomCost != searchRoomCost)
                    {
                        prebookSuccess = false;
                        errorType = "Room Price Does Not Match";
                        break;
                    }

                    count += 1;
                }
            }

            if (!prebookSuccess)
            {
                propertyDetails.Warnings.AddNew("PreBook", errorType);
            }

            return prebookSuccess;
        }

        private async Task<bool> BookingTermsRequestAsync(PropertyDetails propertyDetails)
        {
            var webRequest = new Request();
            bool success;

            try
            {
                var envelope = HotelstonHelper.CreateEnvelope<BookingTermsRequest>(_settings, propertyDetails);
                var request = envelope.Body.Content;

                request.HotelId = propertyDetails.TPKey;
                request.SearchId = propertyDetails.Rooms[0].ThirdPartyReference.Split('|')[1];

                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    string[] roomTypeCodeItems = roomDetails.RoomTypeCode.Split('|');
                    roomDetails.ThirdPartyReference += ( "|" + roomDetails.RoomTypeCode);

                    request.Rooms.Add(new Models.Common.Room
                    {
                        RoomId = roomTypeCodeItems[0],
                        RoomTypeId = roomTypeCodeItems[1],
                        BoardTypeId = roomDetails.ThirdPartyReference.Split('|')[2]
                    });
                }

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.HOTELSTON,
                    LogFileName = "BookingTermsRequest",
                    CreateLog = true,
                    ExtraInfo = propertyDetails,
                    SoapAction = "urn:getBookingTerms",
                    UseGZip = true,
                    SOAP = true
                };

                webRequest.SetRequest(HotelstonHelper.Serialize(envelope, envelope.Xmlns, envelope.AttributeOverrides));
                await webRequest.Send(_httpClient, _logger);

                var response = HotelstonHelper.DeSerialize<BookingTermsResponse>(webRequest.ResponseXML, _serializer);

                success = response.Success;

                if (success)
                {
                    propertyDetails.LocalCost = propertyDetails.Rooms.Sum(r => r.LocalCost);
                    ProcessBookingTerms(response, propertyDetails);
                }
            }
            catch (Exception ex)
            {
                success = false;
                propertyDetails.Warnings.AddNew("Hotelston Booking Terms Exception", ex.Message);
            }
            finally
            {
                propertyDetails.AddLog("Booking Terms", webRequest);
            }

            return success;
        }

        private static void ProcessBookingTerms(BookingTermsResponse response, PropertyDetails propertyDetails)
        {
            var errata = new Errata();

            foreach (var bookingTerms in response.BookingTerms)
            {
                errata.AddNew("Booking Remark", bookingTerms.BookingRemarks);
            }

            propertyDetails.Errata.AddRange(errata);
        }

        public async Task<string> BookAsync(PropertyDetails propertyDetails)
        {
            string[] titles = { "MR", "MRS", "MS", "MISS" };
            string reference = "failed";
            var webRequest = new Request();

            string title = propertyDetails.LeadGuestTitle.ToUpper();
            string firstName = propertyDetails.LeadGuestFirstName;
            string lastName = propertyDetails.LeadGuestLastName;
            string email = _settings.ContactEmail(propertyDetails);
            string phone = propertyDetails.LeadGuestPhone;
            string leadGuestEmail = propertyDetails.LeadGuestEmail;
            bool useDefaultEmail = _settings.AlwaysUseDefaultEmail(propertyDetails);

            if (!titles.Contains(title))
                title = "MR";
            if (string.IsNullOrWhiteSpace(firstName))
                firstName = _settings.DefaultFirstName(propertyDetails);
            if (string.IsNullOrWhiteSpace(lastName))
                lastName = _settings.DefaultLastName(propertyDetails);
            if (!useDefaultEmail && !string.IsNullOrWhiteSpace(leadGuestEmail))
            {
                email = leadGuestEmail;
            }

            if (string.IsNullOrWhiteSpace(phone))
                phone = _settings.ContactPhoneNumber(propertyDetails);
            try
            {
                var envelope = HotelstonHelper.CreateEnvelope<BookHotelRequest>(_settings, propertyDetails);
                var request = envelope.Body.Content;

                request.CheckIn = propertyDetails.ArrivalDate.ToString(HotelstonHelper.DateFormatString);
                request.CheckOut = propertyDetails.DepartureDate.ToString(HotelstonHelper.DateFormatString);
                request.HotelId = propertyDetails.TPKey;
                request.AgentReferenceNumber = propertyDetails.BookingReference;
                request.ConfirmedBooking = "true";
                request.ContactPerson.Title = title;
                request.ContactPerson.Firstname = firstName;
                request.ContactPerson.Lastname = lastName;
                request.ContactPerson.Email = email;
                request.ContactPerson.Phone = phone;

                foreach (var roomDetails in propertyDetails.Rooms)
                {
                    string[] roomTypeCodeItems = roomDetails.RoomTypeCode.Split('|');
                    // use LocalCost because GrossCost is empty
                    decimal roomCost = Math.Round(roomDetails.LocalCost.ToSafeDecimal(), 2, MidpointRounding.AwayFromZero);

                    var room = new BookRoom
                    {
                        // ThirdPartyReference 0 - hotelId 1 - SearchId, 2 - searchBoardTypeId, 3 - RoomId, 4 - RoomTypeId
                        RoomId = roomDetails.ThirdPartyReference.Split('|')[3],
                        RoomTypeId = roomDetails.ThirdPartyReference.Split('|')[4],
                        BoardTypeId = roomDetails.ThirdPartyReference.Split('|')[2],
                        Price = roomCost
                    };

                    request.Rooms.Add(room);
                    
                    foreach (var passenger in roomDetails.Passengers.Where(o => o.Age == 0))
                        passenger.Age = GetAgeFromDob(passenger.DateOfBirth);

                    room.Adults = (from passenger in roomDetails.Passengers
                        where passenger.Age > 14
                        let passengerTitle = passenger.Title.ToUpper()
                        select new Guest
                        {
                            Title = titles.Contains(passengerTitle) ? passengerTitle : "MR",
                            Firstname = passenger.FirstName,
                            Lastname = passenger.LastName
                        }).ToArray();

                    room.Children = (from passenger in roomDetails.Passengers
                        where passenger.Age <= 14
                        select new Guest
                        {
                            Firstname = passenger.FirstName,
                            Lastname = passenger.LastName,
                            Age = passenger.Age
                        }).ToArray();
                }

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.HOTELSTON,
                    LogFileName = "Book",
                    CreateLog = true,
                    ExtraInfo = propertyDetails,
                    SoapAction = "urn:bookHotel",
                    UseGZip = true,
                    SOAP = true
                };

                webRequest.SetRequest(HotelstonHelper.Serialize(envelope, envelope.Xmlns, envelope.AttributeOverrides));
                await webRequest.Send(_httpClient, _logger);

                var response = HotelstonHelper.DeSerialize<BookHotelResponse>(webRequest.ResponseXML, _serializer);

                if (response.Success)
                {
                    reference = response.BookingReference;
                }
            }
            catch (Exception ex)
            {
                propertyDetails.Warnings.AddNew("Hotelston Book Exception", ex.Message);
            }
            finally
            {
                propertyDetails.AddLog("Book", webRequest);
            }

            return reference;
        }

        private static int GetAgeFromDob(DateTime passengerDateOfBirth)
        {
            long dobTicks = passengerDateOfBirth.Ticks;
            long todayTicks = DateTime.Now.Ticks;
            long elapsedTicks = todayTicks - dobTicks;
            var age = new DateTime(elapsedTicks);
            return age.Year;
        }

        public async Task<ThirdPartyCancellationResponse> CancelBookingAsync(PropertyDetails propertyDetails)
        {
            var thirdPartyCancellationResponse = new ThirdPartyCancellationResponse();
            var webRequest = new Request();

            try
            {
                var envelope = HotelstonHelper.CreateEnvelope<CancelHotelBookingRequest>(_settings, propertyDetails);
                var request = envelope.Body.Content;

                request.BookingReference = propertyDetails.SourceReference;

                webRequest = new Request
                {
                    EndPoint = _settings.GenericURL(propertyDetails),
                    Method = RequestMethod.POST,
                    Source = ThirdParties.HOTELSTON,
                    LogFileName = "CancelHotelBookingRequest",
                    CreateLog = true,
                    ExtraInfo = propertyDetails,
                    SoapAction = "urn:CancelHotelBookingRequest",
                    UseGZip = true,
                    SOAP = true
                };
                webRequest.SetRequest(HotelstonHelper.Serialize(envelope, envelope.Xmlns, envelope.AttributeOverrides));
                await webRequest.Send(_httpClient, _logger);

                var response = HotelstonHelper.DeSerialize<CancelHotelBookingResponse>(webRequest.ResponseXML, _serializer);

                thirdPartyCancellationResponse.Amount = response.CancellationFee;
                thirdPartyCancellationResponse.Success = response.Success;
            }
            catch (Exception ex)
            {
                thirdPartyCancellationResponse.Success = false;
                propertyDetails.Warnings.AddNew("Hotelston Cancellation Exception", ex.Message);
            }
            finally
            {
                propertyDetails.AddLog("Cancellation", webRequest);
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
            throw new NotImplementedException();
        }

        public string CreateReconciliationReference(string inputReference)
        {
            return string.Empty;
        }

        public void EndSession(PropertyDetails propertyDetails)
        {
        }

        public class Room
        {
            public int Adults { get; set; }

            public int Children { get; set; }

            public List<int> ChildAges { get; set; } = new();

            public Room(RoomDetail room)
            {
                CalculatePassengersByAge(room.Adults, room.Infants, room.ChildAges);
            }

            public Room(RoomDetails room)
            {
                CalculatePassengersByAge(room.Adults, room.Infants, room.ChildAges);
            }

            private void CalculatePassengersByAge(int adults, int infants, IReadOnlyCollection<int> childAges)
            {
                var oActualChildAges = childAges.Where(childAge => childAge <= 14).ToList();
                Adults = adults + (childAges.Count - oActualChildAges.Count);
                Children = oActualChildAges.Count + infants;
                ChildAges.AddRange(oActualChildAges);
                for (int i = 1; i <= infants; i++)
                    ChildAges.Add(1);
            }
        }
    }
}
