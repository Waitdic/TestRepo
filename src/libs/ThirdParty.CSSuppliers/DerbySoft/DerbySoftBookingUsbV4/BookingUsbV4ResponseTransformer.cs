namespace ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using Newtonsoft.Json;
    using ThirdParty.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;

    public class BookingUsbV4ResponseTransformer : ISearchResponseTransformer
    {
        private readonly TransformedResultBuilder _resultBuilder;

        public BookingUsbV4ResponseTransformer(IDerbySoftSettings settings, string source)
        {
            _resultBuilder = new TransformedResultBuilder(settings, source);
        }

        public IEnumerable<TransformedResult> TransformResponses(List<Request> requests)
        {
            var responses = new List<Tuple<int, DerbySoftBookingUsbV4AvailabilityResponse>>();
            var hotelList = new List<string>();
            var searchHelper = (SearchExtraHelper)requests.First().ExtraInfo;

            foreach (var request in requests.OrderBy(o => ((SearchExtraHelper)o.ExtraInfo).ExtraInfo.ToSafeInt()))
            {
                var propertyRoomBookingID = ((SearchExtraHelper)request.ExtraInfo).ExtraInfo.ToSafeInt();
                var response = new DerbySoftBookingUsbV4AvailabilityResponse();

                if (!request.Success)
                {
                    continue;
                }

                response = JsonConvert.DeserializeObject<DerbySoftBookingUsbV4AvailabilityResponse>(
                    request.ResponseString, 
                    DerbySoftSupport.GetJsonSerializerSettings());

                // check we have rates
                if (!response.RoomRates.Any())
                {
                    continue;
                }

                if (!hotelList.Contains(response.HotelId)) hotelList.Add(response.HotelId);

                responses.Add(new Tuple<int, DerbySoftBookingUsbV4AvailabilityResponse>(propertyRoomBookingID, response));
            }

            var validHotels = HotelValidator.HotelsWithCompleteRoomSelection(hotelList, searchHelper.SearchDetails.Rooms, responses);

            return responses.Where(r => validHotels.Contains(r.Item2.HotelId))
                .SelectMany(r => GetResultFromResponse(searchHelper.SearchDetails, r));
        }

        private IEnumerable<TransformedResult> GetResultFromResponse(SearchDetails searchDetails, Tuple<int, DerbySoftBookingUsbV4AvailabilityResponse> roomResponse)
        {
            var transformedResults = new List<TransformedResult>();

            foreach (var roomRate in roomResponse.Item2.RoomRates)
            {
                var transformedResult =
                    _resultBuilder.BuildTransformedResult(
                        searchDetails,
                        roomResponse.Item1,
                        roomResponse.Item2.HotelId,
                        roomResponse.Item2.Header.Token,
                        roomRate);

                if (transformedResult is null)
                {
                    continue;
                }

                transformedResults.Add(transformedResult);

            }

            return transformedResults;
        }
    }
}
