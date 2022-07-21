namespace iVectorOne.CSSuppliers.Acerooms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Newtonsoft.Json;
    using iVectorOne;
    using iVectorOne.Constants;
    using iVectorOne.CSSuppliers.AceRooms.Models;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using static iVectorOne.CSSuppliers.AceRooms.Models.AceroomsAvailabilityRequest;

    public class AceroomsSearch : IThirdPartySearch, ISingleSource
    {
        #region Properties

        private readonly IAceroomsSettings _settings;
        private readonly ITPSupport _support;

        public string Source => ThirdParties.ACEROOMS;

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        #endregion

        #region Constructors

        public AceroomsSearch(IAceroomsSettings settings, ITPSupport support)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        #endregion

        #region Build Search Request

        public async Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var requests = new List<Request>();
            var hotelIDList = new List<int>();
            var batchLimit = _settings.HotelBatchLimit(searchDetails);

            // Build request for each city
            foreach (var searchResortSplit in resortSplits)
            {
                foreach (var oHotel in searchResortSplit.Hotels)
                {
                    if (hotelIDList.Count < batchLimit) // Limit number of hotels per request
                    {
                        hotelIDList.Add(oHotel.TPKey.ToSafeInt());
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
                    requests.Add(await BuildRequestAsync(searchResortSplit.ResortCode.ToSafeInt(), searchDetails, batch));
                }
            }

            return requests;
        }

        #endregion

        #region Transform Response

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
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

        #region Helper classes

        public List<TransformedResult> GetResultFromResponse(AceroomsAvailabilityResponse response, SearchDetails searchDetails)
        {
            var transformedResults = new List<TransformedResult>();

            foreach (var hotel in response.Hotels)
            {
                foreach (var room in hotel.Rooms)
                {
                    foreach (var rate in room.Rates)
                    {
                        var isNonRefundable = rate.CancelPolicy?.Description == "Non Refundable";
                        var transformedResult = new TransformedResult()
                        {
                            MealBasisCode = rate.BoardID.ToString(),
                            RoomType = rate.Room,
                            Amount = rate.Rate,
                            SellingPrice = rate.Rate,
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
        private async Task<Request> BuildRequestAsync(int resortCode, SearchDetails searchDetails, List<int> hotelIDs)
        {
            var availabilityRequest = await CreateSearchRequestAsync(resortCode, searchDetails, hotelIDs);

            var request = new Request()
            {
                EndPoint = _settings.GenericURL(searchDetails) + "Search",
                Method = RequestMethod.POST,
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
        private async Task<AceroomsAvailabilityRequest> CreateSearchRequestAsync(int resortCode, SearchDetails searchDetails, List<int> hotelIDs)
        {
            var aceroomsAvailabilityRequest = new AceroomsAvailabilityRequest
            {
                CityID = resortCode,
                Hotels = hotelIDs,
                ArrivalDate = searchDetails.ArrivalDate.ToString("yyyy-MM-dd"),
                Nights = searchDetails.Duration,
                CurrencyCode = await _support.TPCurrencyCodeLookupAsync(Source, searchDetails.ISOCurrencyCode)
            };

            var tpNationalityCode = await _support.TPNationalityLookupAsync(Source, searchDetails.ISONationalityCode);

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