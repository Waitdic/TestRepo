﻿namespace iVectorOne.Suppliers.TBOHolidays
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Net;
    using Lookups;
    using Models;
    using Models.Common;
    using Models.Search;
    using iVectorOne.Constants;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.SDK.V2.PropertySearch;
    using Newtonsoft.Json;
    using iVectorOne.Models.Property.Booking;
    using Intuitive.Helpers.Extensions;

    public class TBOHolidaysSearch : IThirdPartySearch, ISingleSource
    {
        private const string DateFormat = "yyyy-MM-dd";

        private readonly ITBOHolidaysSettings _settings;
        private readonly ITPSupport _support;

        public TBOHolidaysSearch(ITBOHolidaysSettings settings, ITPSupport support)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        public string Source => ThirdParties.TBOHOLIDAYS;

        public async Task<List<Intuitive.Helpers.Net.Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            if (resortSplits.Count == 0)
            {
                return new List<Intuitive.Helpers.Net.Request>();
            }

            var requests = new List<Intuitive.Helpers.Net.Request>();
            var auth = Helper.GetAuth(_settings.User(searchDetails), _settings.Password(searchDetails));

            for (var index = 0; index < searchDetails.Rooms; index++)
            {
                var request = await BuildSearchRequestAsync(searchDetails, resortSplits, index);
                
                var webRequest = new Intuitive.Helpers.Net.Request
                {
                    EndPoint = _settings.SearchURL(searchDetails),
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Application_json,
                    Accept = "application/json",
                };

                webRequest.Headers.AddNew("Authorization", auth);
                webRequest.SetRequest(request);
                requests.Add(webRequest);
            }

            return requests;
        }

        private async Task<string> BuildSearchRequestAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits, int index)
        {
            var guestNationality = string.Empty;

            if (!string.IsNullOrEmpty(searchDetails.ISONationalityCode))
            {
                guestNationality = await _support.TPNationalityLookupAsync(Source, searchDetails.ISONationalityCode);
            }

            if (string.IsNullOrEmpty(guestNationality))
            {
                guestNationality = _settings.LeadGuestNationality(searchDetails);
            }

            var request = new HotelSearchWithRoomsRequest
            {
                CheckIn = searchDetails.ArrivalDate.ToString(DateFormat),
                CheckOut = searchDetails.DepartureDate.ToString(DateFormat),
                HotelCodes = string.Join(',', resortSplits.SelectMany(x => x.Hotels).Select(x => x.TPKey)),
                GuestNationality = guestNationality,
                ResponseTime = 23.0m,
                IsDetailedResponse = true,
                Filters =
                {
                    Refundable = _settings.ExcludeNRF(searchDetails),
                    NoOfRooms = _settings.RoomSearchLimit(searchDetails),
                    MealType = _settings.RequestedMealBases(searchDetails)
                }
            };

            var roomDetail = searchDetails.RoomDetails[index];
            var paxRoom = new PaxRoom
            {
                Adults = roomDetail.Adults,
                Children = roomDetail.Children + roomDetail.Infants,
            };

            if (paxRoom.Children > 0)
            {
                paxRoom.ChildrenAges.AddRange(roomDetail.ChildAges);
                for (var i = 0; i < roomDetail.Infants; i++)
                {
                    paxRoom.ChildrenAges.Add(0);
                }
            }

            request.PaxRooms.Add(paxRoom);
            
            return JsonConvert.SerializeObject(request);
        }

        public TransformedResultCollection TransformResponse(List<Intuitive.Helpers.Net.Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformed = new TransformedResultCollection();

            var hotelResponses = requests
                .Select(request => JsonConvert.DeserializeObject<HotelResponse>(request.ResponseString))
                .Where(x => Helper.CheckStatus(x.Status))
                .ToList();

            if (hotelResponses.Count != searchDetails.Rooms)
            {
                return transformed;
            }

            transformed.TransformedResults.AddRange(BuildTransformResult(hotelResponses!, searchDetails));
            return transformed;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        public bool ResponseHasExceptions(Intuitive.Helpers.Net.Request request)
        {
            return false;
        }

        private List<TransformedResult> BuildTransformResult(List<HotelResponse> result, SearchDetails searchDetails)
        {
            var transformedResult = new List<TransformedResult>();

            foreach (var hotelCode in result
                         .SelectMany(x => x.HotelResult)
                         .GroupBy(hr => hr.HotelCode)
                         .Where(x => x.Count() == result.Count)
                         .Select(g => g.Key))
            {
                for (var prbid = 0; prbid < result.Count; prbid++)
                {
                    foreach (var hotelResult in result[prbid].HotelResult.Where(hr => hr.HotelCode == hotelCode))
                    {
                        foreach (var room in hotelResult.Rooms)
                        {
                            var supplements = room.Supplements
                                    .SelectMany(x => x.Where(s => s.Type == SuppChargeType.AtProperty)
                                    .ToList());

                            var supplementInformation = string.Concat(supplements.Select(c => $"{c.Index},{c.Description},{c.Price},{c.Currency}|"));
                            var tpReference = string.Concat(room.BookingCode, Helper.Separators[0],
                                supplementInformation);

                            var cancellationList = new Cancellations();
                            foreach (var policyNode in room.CancelPolicies)
                            {
                                var amount = policyNode.ChargeType switch
                                {
                                    ChargeType.Fixed => policyNode.CancellationCharge,
                                    ChargeType.Percentage => (policyNode.CancellationCharge / 100) * room.TotalFare,
                                    _ => 0
                                };

                                var toDate = room.CancelPolicies.Any(c => c.FromDate.ToSafeDate() > policyNode.FromDate.ToSafeDate())
                                    ? room.CancelPolicies
                                        .Where(c => c.FromDate.ToSafeDate() > policyNode.FromDate.ToSafeDate())
                                        .Select(x => x.FromDate.ToSafeDate())
                                        .OrderBy(x => x)
                                        .First()
                                        .AddDays(-1)
                                    : searchDetails.ArrivalDate;

                                cancellationList.AddNew(policyNode.FromDate.ToSafeDate(), toDate, amount);
                            }

                            transformedResult.Add(new TransformedResult
                            {
                                TPKey = hotelResult.HotelCode,
                                CurrencyCode = hotelResult.Currency,
                                PropertyRoomBookingID = prbid + 1,
                                RoomType = room.Name.First(),
                                MealBasisCode = room.MealType,
                                Amount = room.TotalFare,
                                TPReference = tpReference,
                                MinimumPrice = room.RecommendedSellingRate,
                                Cancellations = cancellationList,
                                Adjustments = room.RoomPromotion
                                    .Select(s => new TransformedResultAdjustment(
                                        AdjustmentType.Offer,
                                        s,
                                        string.Empty,
                                        0)).ToList()
                            });
                        }
                    }
                }
            }

            return transformedResult;
        }
    }
}