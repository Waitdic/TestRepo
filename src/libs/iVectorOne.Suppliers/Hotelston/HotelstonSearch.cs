namespace ThirdParty.CSSuppliers.Hotelston
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.Hotelston.Models;
    using ThirdParty.CSSuppliers.Hotelston.Models.Common;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;

    public class HotelstonSearch : IThirdPartySearch, ISingleSource
    {
        private readonly IHotelstonSettings _settings;
        private readonly ISerializer _serializer;

        public HotelstonSearch(IHotelstonSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public string Source => ThirdParties.HOTELSTON;

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            return Task.FromResult(resortSplits.Select(resortSplit => BuildRequest(searchDetails, resortSplit)).ToList());
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();

            var allResponses =
                from request in requests
                where request.Success
                select HotelstonHelper.DeSerialize<SearchHotelsResponse>(request.ResponseXML, _serializer);

            transformedResults.TransformedResults.AddRange(
                allResponses.Where(r => r.Success)
                    .SelectMany(r => GetResultFromResponse(r, searchDetails)));

            return transformedResults;
        }

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            if (searchDetails.RoomDetails.Count > 3)
            {
                return true;
            }

            foreach (var roomDetail in searchDetails.RoomDetails)
            {
                var room = new Hotelston.Room(roomDetail);

                if (room.ChildAges.Count > 3)
                {
                    return true;
                }

                if (room.Adults > 4)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        private IEnumerable<TransformedResult> GetResultFromResponse(SearchHotelsResponse response, SearchDetails searchDetails)
        {
            return (
                from hotel in response.Hotels
                from room in hotel.Rooms
                select new TransformedResult
                {
                    TPKey = hotel.Id,
                    CurrencyCode = _settings.Currency(searchDetails),
                    PropertyRoomBookingID = room.SeqNo + 1,
                    RoomType = room.RoomType.NameEn,
                    RoomTypeCode = $"{room.Id}|{room.RoomType.Id}",
                    MealBasisCode = room.BoardType.Id,
                    Amount = room.Price,
                    SpecialOffer = room.SpecialOffer
                        ? string.Join(".  ", room.SpecifficSpecialOffers.Select(specialOffer => specialOffer.Details))
                        : string.Empty,
                    TPReference = $"{hotel.Id}|{response.SearchId}|{room.BoardType.Id}",
                }).ToList();
        }

        private Request BuildRequest(SearchDetails searchDetails, IResortSplit resortSplit)
        {
            var envelope = HotelstonHelper.CreateEnvelope<SearchHotelsRequest>(_settings, searchDetails);
            var request = envelope.Body.Content;

            request.Criteria.CheckIn = searchDetails.ArrivalDate.ToString(HotelstonHelper.DateFormatString);
            request.Criteria.CheckOut = searchDetails.DepartureDate.ToString(HotelstonHelper.DateFormatString);

            if (resortSplit.Hotels.Count > 1)
            {
                request.Criteria.CityId = resortSplit.ResortCode;
            }
            else
            {
                request.Criteria.HotelId = resortSplit.Hotels[0].TPKey;
            }

            foreach (var room in searchDetails.RoomDetails.Select(roomDetail => new Hotelston.Room(roomDetail)))
            {
                request.Criteria.Rooms.Add(new Room
                {
                    Adults = room.Adults,
                    Children = room.Children,
                    ChildAges = room.ChildAges.ToArray()
                });
            }

            var webRequest = new Request
            {
                EndPoint = _settings.EndPointUrl(searchDetails),
                Method = RequestMethod.POST,
                SoapAction = "urn:searchHotels",
                SOAP = true
            };

            webRequest.SetRequest(HotelstonHelper.Serialize(envelope, envelope.Xmlns, envelope.AttributeOverrides));
            return webRequest;
        }
    }
}