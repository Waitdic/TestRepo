namespace iVectorOne.Suppliers.FastPayHotels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using Newtonsoft.Json;
    using iVectorOne.Search.Models;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.FastPayHotels.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Search.Results.Models;
    using Cancellation = iVectorOne.Models.Cancellation;
    using RoomDetails = iVector.Search.Property.RoomDetails;
    using static iVectorOne.Suppliers.FastPayHotels.Models.SharedModels;
    using static iVectorOne.Suppliers.FastPayHotels.Models.FastPayHotelsAvailabilityResponse;
    using static iVectorOne.Suppliers.FastPayHotels.Models.FastPayHotelsAvailabilityRequest;
    using iVectorOne.Models.Property;

    public class FastPayHotelsSearch : IThirdPartySearch, ISingleSource
    {
        #region Constructors

        private readonly IFastPayHotelsSettings _settings;

        private readonly ITPSupport _support;

        public FastPayHotelsSearch(IFastPayHotelsSettings settings, ITPSupport support)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        #endregion

        #region Properties

        public string Source => ThirdParties.FASTPAYHOTELS;

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        #endregion

        #region Build search request

        public async Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();

            var messageId = Guid.NewGuid().ToSafeString();
            var availabilityRequest = await CreateAvailabilityRequestAsync(searchDetails, searchDetails.RoomDetails, messageId, resortSplits);
            string requestString = JsonConvert.SerializeObject(availabilityRequest);

            var availabilityUrl = _settings.AvailabilityURL(searchDetails) + "api/booking/availability";
            var request = FastPayHotels.CreateWebRequest(availabilityUrl, "Search ", searchDetails, ContentTypes.Application_json, _settings, requestString);

            requests.Add(request);

            return requests;
        }

        #endregion

        #region Transform response

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var responses = new List<FastPayHotelsAvailabilityResponse>();

            foreach (var request in requests)
            {
                var response = new FastPayHotelsAvailabilityResponse();
                bool success = request.Success;

                if (success)
                {
                    response = JsonConvert.DeserializeObject<FastPayHotelsAvailabilityResponse>(request.ResponseString);
                    responses.Add(response);
                }
            }

            transformedResults.TransformedResults.AddRange(responses.Where(o => o.hotelAvails.Count > 0).SelectMany(r => GetResultFromResponse(searchDetails, r)));

            return transformedResults;
        }

        #endregion

        #region Helper classes

        public async Task<FastPayHotelsAvailabilityRequest> CreateAvailabilityRequestAsync(SearchDetails searchDetails, RoomDetails roomDetails, string guid, List<ResortSplit> resortSplits)
        {
            return new FastPayHotelsAvailabilityRequest
            {
                messageID = guid,
                currency = _settings.UseCurrencyCode(searchDetails) ? searchDetails.ISOCurrencyCode : string.Empty,
                checkIn = searchDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                checkOut = searchDetails.DepartureDate.ToString("yyyy-MM-dd"),
                occupancies = GetOccupancies(roomDetails),
                hotelCodes = GetHotelCodes(resortSplits),
                parameters = await GetParametersAsync(searchDetails, _settings)
            };
        }

        public static List<Occupancy> GetOccupancies(RoomDetails roomDetails)
        {
            var occupancies = new List<Occupancy>();

            foreach (var room in roomDetails)
            {
                occupancies.Add(new Occupancy()
                {
                    adults = room.Adults,
                    children = room.Children,
                    childrenAges = room.ChildAges
                });
            }

            return occupancies;
        }

        public static List<string> GetHotelCodes(List<ResortSplit> resortSplits)
        {
            var hotelCodes = new List<string>();

            foreach (ResortSplit resort in resortSplits)
            {
                foreach (Hotel hotel in resort.Hotels)
                {
                    hotelCodes.Add(hotel.TPKey);
                }
            }

            return hotelCodes;
        }

        private List<TransformedResult> GetResultFromResponse(SearchDetails searchDetails, FastPayHotelsAvailabilityResponse response)
        {
            List<TransformedResult> transformedResults = new List<TransformedResult>();

            List<int> roomIds = new List<int>();
            if (searchDetails.Rooms > 1)
            {
                GetRoomIds(response.hotelAvails, roomIds, searchDetails); // match returned room with roomBookings ids 
            }
            else
            {
                roomIds.Add(1); // set roomBooking to 1 if single room
            }

            int roomCounter = 0;
            foreach (var hotel in response.hotelAvails)
            {
                foreach (var room in hotel.availRoomRates)
                {
                    var totalCost = GetTotalCost(room, searchDetails);
                    TransformedResult transformedResult = new TransformedResult()
                    {
                        RoomType = room.roomName,
                        PropertyRoomBookingID = roomIds[roomCounter],
                        MealBasisCode = room.mealPlanCode,
                        TPKey = hotel.hotelinfo.code,
                        TPReference = room.reservationToken,
                        CurrencyCode = room.currency,
                        Amount = totalCost,
                        NonRefundableRates = !room.cancellationPolicy.cancellable,
                        Cancellations = GetCancellations(room.cancellationPolicy, searchDetails, totalCost)
                        //TODO: Set SellingPrice 
                        //TODO: Set LocalCost
                    };

                    roomCounter = searchDetails.Rooms == 1 ? 0 : roomCounter + 1;
                    transformedResults.Add(transformedResult);
                }
            }

            return transformedResults;
        }

        private Cancellations GetCancellations(CancellationPolicy cancellationPolicy, SearchDetails searchDetails, decimal totatlCost)
        {

            var cancellations = new Cancellations();
            if (!cancellationPolicy.cancellable || cancellationPolicy.code == "100P_AD100P") // code for non refundable
            {
                cancellations.Add(new Cancellation()
                {
                    Amount = totatlCost,
                    StartDate = DateTime.Now,
                    EndDate = new DateTime(2099, 1, 1)
                });

                return cancellations;
            }
            else if (!string.IsNullOrEmpty(cancellationPolicy.code))
            {
                cancellations = GetCancellationFromCode(cancellationPolicy.code, searchDetails, totatlCost);
            }

            // Fastpayhotels is still working on codyfying the penalty rule. Use following commented out code instead of above else if they have completed the implementation
            //else if (cancellationPolicy.penalties != null && cancellationPolicy.penalties.Count > 0)
            //{
            //    //foreach (var penalty in cancellationPolicy.penalties)
            //    //{
            //    //    var cancellation = CalculateCallation(penalty, searchDetails);
            //    //    cancellations.Add(cancellation);
            //    //}
            //}

            return cancellations;

        }

        Cancellations GetCancellationFromCode(string penaltyString, SearchDetails searchDetails, decimal stayCost)
        {
            var cancellations = new Cancellations();
            if (!string.IsNullOrEmpty(penaltyString))
            {
                // penaltyString is a string witch multiple peanlty codes seperated by _. The first penalty rule is always closest to the arrival date
                bool contaisMultipleCodes = penaltyString.IndexOf('_') != -1;

                List<string> penaltyRules = !contaisMultipleCodes ? new List<string> { penaltyString } : penaltyString.Split('_').ToList();

                int deadlineTime = 0;

                if (penaltyRules[penaltyRules.Count() - 1].Contains("#")) // look whether it includes deadline time details
                {
                    // the cancellation deadline time is returned as last string separated by _ or null  in the form of AD0#9PM
                    Tuple<int, string> deadline = ExtractNumberAndString(penaltyRules[penaltyRules.Count() - 1].Split('#')[1]); // deadline time is stored in the form of i.e. 9, AM
                    deadlineTime = deadline.Item2 == "PM" ? deadline.Item1 + 12 : deadline.Item1;
                    penaltyRules[penaltyRules.Count() - 1] = penaltyRules[penaltyRules.Count() - 1].Split('#')[0]; // remove the deadline time out of last penalty rule
                }
                else
                {
                    deadlineTime = 0; // set the deadlineTime to 12 AM as default
                }

                var lastStartDate = new DateTime();

                for (int i = 0; i < penaltyRules.Count(); ++i)
                {
                    cancellations.Add(CalculateChargeBase(penaltyRules[i], ref lastStartDate, stayCost, searchDetails, deadlineTime));
                }
            }

            return cancellations;
        }

        Cancellation CalculateChargeBase(string penaltyRule, ref DateTime lastStartDate, decimal stayCost, SearchDetails searchDetails, int deadlineTime)
        {
            int average = penaltyRule.IndexOf('A');
            if (average != -1) { penaltyRule = penaltyRule.Remove(average, 3); }

            string[] freeCancellationCodes = { "0P", "AD0_0", "AD0P_0P" };
            Cancellation cancellation = new Cancellation();

            if (penaltyRule.Count() == 1 && penaltyRule.Equals("0") || freeCancellationCodes.Any(s => penaltyRule.Equals(s))) // assumes that the free cancellation policy always starts on booking date until a certain day before arrival
            {
                cancellation.StartDate = DateTime.Now;
                cancellation.EndDate = lastStartDate;
                cancellation.Amount = 0;
            }
            else
            {
                int offSetTimeValueFromArrival = 0;
                int daysBeforeArrival = penaltyRule.IndexOf('D');
                var offsetimeValue = lastStartDate == default(DateTime) ? 0 : deadlineTime;


                if (daysBeforeArrival != -1) // contains information about cancellation start date before arrival
                {
                    offSetTimeValueFromArrival = ExtractNumberAndString(penaltyRule.Substring(0, daysBeforeArrival)).Item1;
                    cancellation.StartDate = searchDetails.ArrivalDate.AddDays(-offSetTimeValueFromArrival).AddHours(deadlineTime);
                }
                else if (penaltyRule.Contains("H"))
                {
                    offSetTimeValueFromArrival = ExtractNumberAndString(penaltyRule.Substring(0, daysBeforeArrival)).Item1;
                    cancellation.StartDate = searchDetails.ArrivalDate.AddHours(offSetTimeValueFromArrival);
                }
                else // if no start date deatils then assume no show 
                {
                    cancellation.StartDate = searchDetails.ArrivalDate.Date.AddDays(1).AddHours(deadlineTime);
                }

                if (penaltyRule.Contains("P")) // percentage charge base
                {
                    int percentage = ExtractNumberAndString(penaltyRule.Substring(daysBeforeArrival == -1 ? 0 : daysBeforeArrival)).Item1;
                    cancellation.Amount = stayCost * ((decimal)percentage / 100m);
                }
                else if (penaltyRule.Contains("N")) // night charge base 
                {
                    int numOfNights = ExtractNumberAndString(penaltyRule.Substring(daysBeforeArrival == -1 ? 0 : daysBeforeArrival)).Item1;
                    cancellation.Amount = (stayCost / (decimal)searchDetails.Duration) * numOfNights;
                }

                lastStartDate = lastStartDate == default(DateTime) ? searchDetails.ArrivalDate.AddHours(offsetimeValue) : lastStartDate;
                cancellation.EndDate = daysBeforeArrival == -1 ? new DateTime(2099, 1, 1) : lastStartDate.AddMilliseconds(-1);
                lastStartDate = cancellation.StartDate;
            }

            return cancellation;
        }

        Tuple<int, string> ExtractNumberAndString(string input)
        {
            string s = string.Empty;
            int num = 0;

            if (!string.IsNullOrEmpty(input))
            {
                var tempNum = string.Empty;
                foreach (var d in input)
                {
                    if (Char.IsDigit(d))
                    {
                        tempNum += d;
                    }
                    else
                    {
                        s += d;
                    }
                }

                num = tempNum == string.Empty ? 0 : tempNum.ToSafeInt();
            }

            return new Tuple<int, string>(num, s);
        }

        Cancellation CalculateCallation(Penalty penalty, SearchDetails searchDetails)
        {
            var cancellation = new Cancellation();
            var cancellationDeadline = penalty.cancelDeadLine;

            if (!cancellationDeadline.noShow)
            {
                if (cancellationDeadline.offsetTimeDropType.Equals("Before arrival"))
                {
                    if (cancellationDeadline.offsetTimeUnit.Equals("Days"))
                    {
                        cancellation.StartDate = searchDetails.ArrivalDate.AddDays(-cancellationDeadline.offsetTimeValue);
                    }
                    else if (cancellationDeadline.offsetTimeUnit.Equals("Hours"))
                    {
                        cancellation.StartDate = searchDetails.ArrivalDate.AddHours(-cancellationDeadline.offsetTimeValue);
                    }
                }
                cancellation.EndDate = searchDetails.ArrivalDate.AddDays(1);
            }
            else
            {
                // if no show then set the start date 6 am after check in 
                cancellation.StartDate = searchDetails.ArrivalDate.AddHours(6);
                cancellation.EndDate = new DateTime(2099, 1, 1);
            }

            cancellation.Amount = penalty.penaltyCharge.amount;
            return cancellation;
        }

        void GetRoomIds(List<HotelAvail> hotels, List<int> roomIds, SearchDetails searchDetails)
        {
            foreach (var hotel in hotels)
            {
                foreach (var room in hotel.availRoomRates)
                {
                    for (int i = 0; i < searchDetails.Rooms; ++i)
                    {
                        if (IsSameRoom(room.occupancy, searchDetails.RoomDetails[i]))
                        {
                            roomIds.Add(i + 1);
                        }
                    }
                }
            }
        }

        bool IsSameRoom(Occupancy occupancy, RoomDetail roomDetail)
        {
            return occupancy.adults == roomDetail.Adults &&
                   occupancy.children == roomDetail.Children + roomDetail.Infants &&
                   occupancy.childrenAges.All(roomDetail.ChildAges.Contains);
        }

        decimal GetTotalCost(AvailRoomRate room, SearchDetails searchDetails)
        {
            if (_settings.UserRateType(searchDetails) == "Net") // public 
            {
                if (room.priceBinding)
                {
                    return room.publicPrice;
                }
                else
                {
                    return room.totalPrice;
                }
            }
            else
            {
                return room.totalPrice; // gross(comission)
            }
        }

        private async Task<Parameters> GetParametersAsync(SearchDetails searchDetails, IFastPayHotelsSettings settings)
        {
            return new Parameters
            {
                countryOfResidence = string.IsNullOrEmpty(searchDetails.SellingCountry) ?
                    settings.CountryOfResidence(searchDetails) :
                    await _support.TPCountryCodeLookupAsync(this.Source, searchDetails.SellingCountry, searchDetails.Account.AccountID),
                nationality = string.IsNullOrEmpty(searchDetails.ISONationalityCode) ?
                    settings.LeadGuestNationality(searchDetails) :
                    await _support.TPNationalityLookupAsync(this.Source, searchDetails.ISONationalityCode)
            };
        }

        #endregion
    }
}