namespace ThirdParty.CSSuppliers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.AceRooms.Models;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using ThirdParty.Lookups;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using static ThirdParty.CSSuppliers.AceRooms.Models.AceroomsAvailabilityRequest;

    public class AceroomsSearch : ThirdPartyPropertySearchBase
    {
        #region "Properties"

        private readonly IAceroomsSettings _settings;
        private readonly ITPSupport _support;

        public override bool SqlRequest => false;

        public override string Source => ThirdParties.ACEROOMS;

        public override bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public override bool SearchRestrictions(SearchDetails searchDetails)
        {
            return false;
        }

        #endregion

        #region "Constructors"

        public AceroomsSearch(IAceroomsSettings settings, ITPSupport support, ILogger<AceroomsSearch> logger)
            : base(logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        #endregion

        #region "Build Search Request"

        public override List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs)
        {
            var requests = new List<Request>();
            var hotelIDList = new List<int>();
            var batchLimit = _settings.BatchLimit(searchDetails);

            // Build request for each city
            foreach (var searchResortSplit in resortSplits)
            {
                foreach (var oHotel in searchResortSplit.Hotels)
                {
                    if (hotelIDList.Count < batchLimit) // Limit number of hotels per request
                    {
                        hotelIDList.Add(SafeTypeExtensions.ToSafeInt(oHotel.TPKey));
                    }
                }

                var batches = new List<List<int>>(); // store hotels which exceeds the batch limit
                if (hotelIDList.Count > batchLimit)
                {
                    for (int i = 0; i < hotelIDList.Count; i += batchLimit) // split hotelIds to list of N < batchLimit 
                    {
                        batches.Add(hotelIDList.GetRange(i, Math.Min(batchLimit, hotelIDList.Count - i)));
                    }
                }
                else
                {
                    batches.Add(hotelIDList);
                }

                // Create a request for each batch 
                foreach (var batch in batches)
                {
                    requests.Add(BuildRequest(SafeTypeExtensions.ToSafeInt(searchResortSplit.ResortCode), searchDetails, batch));
                }
            }

            return requests;
        }

        #endregion

        #region "Transform Response"

        public override TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var allResponses = new List<AceroomsAvailabilityResponse>();

            foreach (var request in requests)
            {
                var response = new AceroomsAvailabilityResponse();
                bool success = request.Success;

                if (success)
                {
                    response = JsonConvert.DeserializeObject<AceroomsAvailabilityResponse>(request.ResponseString);
                    if (response != null)
                    {
                        allResponses.Add(response);
                    }
                }
            }

            transformedResults.TransformedResults.AddRange(allResponses.Where(o => o.TotalHotels > 0).SelectMany(r => GetResultFromResponse(r, searchDetails)));

            return transformedResults;
        }

        #endregion

        #region "Helper classes"

        public List<TransformedResult> GetResultFromResponse(AceroomsAvailabilityResponse response, SearchDetails searchDetails)
        {
            List<TransformedResult> transformedResults = new List<TransformedResult>();

            foreach (var hotel in response.Hotels)
            {
                foreach (var room in hotel.Rooms)
                {
                    foreach (var rate in room.Rates)
                    {
                        var isNonRefundable = rate.CancelPolicy?.Description == "Non Refundable";
                        TransformedResult transformedResult = new TransformedResult()
                        {
                            MealBasisCode = rate.BoardID.ToString(),
                            RoomType = rate.Room,
                            Amount = rate.Rate,
                            SellingPrice = rate.Rate.ToString(),
                            MinimumPrice = 0,
                            CurrencyCode = response.SearchInfo.CurrencyCode,
                            TPReference = response.AuditData.SearchToken + "~" + rate.RoomID,
                            TPKey = hotel.HotelID.ToString(),
                            PropertyRoomBookingID = room.RoomNumber,
                            NonRefundableRates = isNonRefundable,
                            Cancellations = GetCancellations(!isNonRefundable, searchDetails.ArrivalDate)
                        };
                        transformedResults.Add(transformedResult);
                    }
                }
            }

            return transformedResults;
        }

        /// <summary>
        /// Heleper class which creates and returns a single web request with given params
        /// </summary>
        /// <param name="resortCode">The unique identider of the city </param>
        /// <param name="searchDetails">The search details</param>
        /// <param name="hotelIDs">The list of all hotel identifiers to search for</param>
        /// <returns></returns>
        private Request BuildRequest(int resortCode, SearchDetails searchDetails, List<int> hotelIDs)
        {
            AceroomsAvailabilityRequest availabilityRequest = CreateSearchRequest(resortCode, searchDetails, hotelIDs);

            var request = new Request()
            {
                EndPoint = _settings.BaseURL(searchDetails) + "Search",
                Method = eRequestMethod.POST,
                Source = Source,
                ContentType = ContentTypes.Application_json,
                ExtraInfo = searchDetails,
                TimeoutInSeconds = 100,
                UseGZip = true
            };

            string requestString = JsonConvert.SerializeObject(availabilityRequest);

            requestString = requestString.Replace(",\"Ages\":[]", string.Empty); // remove the array of child ages in case of no children
            request.SetRequest(requestString);
            request.Headers.AddNew("APIKey", _settings.APIKey(searchDetails));
            request.Headers.AddNew("Signature", _settings.Signature(searchDetails));

            return request;
        }

        /// <summary>
        /// Creates and returns a Acerooms and list of child ages if any
        /// </summary>
        /// <param name="resortCode">the city id</param>
        /// <param name="searchDetails">the search deatails</param>
        /// <param name="hotelIDs">the list of unique hotel identifiers to search for </param>
        /// <returns></returns>
        private AceroomsAvailabilityRequest CreateSearchRequest(int resortCode, SearchDetails searchDetails, List<int> hotelIDs)
        {
            AceroomsAvailabilityRequest aceroomsAvailabilityRequest = new AceroomsAvailabilityRequest
            {
                CityID = resortCode,
                Hotels = hotelIDs,
                ArrivalDate = searchDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                Nights = searchDetails.Duration,
                CurrencyCode = _support.TPCurrencyLookup(Source, searchDetails.CurrencyCode)
            };

            var tpNationalityCode = _support.TPNationalityLookup(Source, searchDetails.NationalityID);

            aceroomsAvailabilityRequest.NationalityID = string.IsNullOrEmpty(tpNationalityCode) ? "1" : tpNationalityCode; // Set to default if nationality id is not provided

            var rooms = new List<Room>();

            foreach (var roomDetail in searchDetails.RoomDetails)
            {
                var room = new Room
                {
                    Adult = roomDetail.Adults,
                    Children = roomDetail.Children + roomDetail.Infants,
                    Ages = roomDetail.ChildAges.ToArray()
                };


                rooms.Add(room);
            }

            aceroomsAvailabilityRequest.Rooms = rooms;

            return aceroomsAvailabilityRequest;
        }

        private List<Cancellation> GetCancellations(bool isFreeCancellaton, DateTime arraivalDate)
        {
            if (isFreeCancellaton)
            {
                return new List<Cancellation> { new Cancellation(DateTime.Now, arraivalDate, 0) };
            }

            return new List<Cancellation>();
        }

        #endregion
    }
}