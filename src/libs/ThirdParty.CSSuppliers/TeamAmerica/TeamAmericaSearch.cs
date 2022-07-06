namespace ThirdParty.CSSuppliers.TeamAmerica
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.CSSuppliers.TeamAmerica.Models;
    using ThirdParty.Models;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;

    public class TeamAmericaSearch : IThirdPartySearch
    {
        #region Properties

        private readonly ITeamAmericaSettings _settings;
        private readonly ISerializer _serializer;

        public string Source => ThirdParties.TEAMAMERICA;

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion

        #region Constructors

        public TeamAmericaSearch(ITeamAmericaSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region SearchFunctions

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails serchDetails, List<ResortSplit> resortSplits)
        {
            var requests = resortSplits.Select(resortSplit =>
            {
                var requestXml = BuildRequestXml(serchDetails, resortSplit);

                //'set a unique code. if the is one request we only need the source name
                string uniqueCode = (resortSplits.Count() > 1)
                                    ? $"{Source}_{resortSplit.ResortCode}"
                                    : Source;

                var request = new Request
                {
                    EndPoint = _settings.URL(serchDetails),
                    SoapAction = $"{_settings.URL(serchDetails)}/{Constant.SoapActionPreBook}",
                    Method = RequestMethod.POST,
                    ContentType = ContentTypes.Text_Xml_charset_utf_8,
                };
                request.SetRequest(requestXml);

                return request;
            }).ToList();

            return Task.FromResult(requests);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var responses = requests.SelectMany(oRequest => Envelope<PriceSearchResponse>
                                        .DeSerialize(oRequest.ResponseXML, _serializer).HotelSearchResponse.Offers).ToList();

            var rooms = searchDetails.RoomDetails.ToArray().Select(roomDetail => new
            {
                PRBID = roomDetail.PropertyRoomBookingID,
                Adults = roomDetail.Adults,
                Children = roomDetail.Children + roomDetail.Infants,
                ChildAges = roomDetail.ChildAndInfantAges()
            });

            var occupancyDict = new Dictionary<int, string>
            {
                { 1, "Single"},
                { 2, "Double"},
                { 3, "Triple"},
                { 4, "Quad"},
            };

            var results = responses
                .Where(hotelOffer => TeamAmerica.IsEveryNightAvailable(hotelOffer) && 
                    !string.IsNullOrEmpty(hotelOffer.RoomType))
                .SelectMany(hotelOffer =>
                    {
                        return rooms.Where(oRoom => oRoom.Adults + oRoom.Children <= hotelOffer.MaxOccupancy)
                        .Select(room =>
                        {
                            int paxCount = string.Equals(hotelOffer.FamilyPlan, Constant.TokenYes)
                            ? room.Adults + room.ChildAges.Where(age => age > hotelOffer.ChildAge).Count()
                            : room.Adults + room.Children;

                            occupancyDict.TryGetValue(paxCount, out string occupancy);

                            decimal amount = hotelOffer.NightlyInfos
                                .Select(info => info.Prices.First(price => string.Equals(price.Occupancy, occupancy)).AdultPrice.ToSafeDecimal())
                                .Sum();

                            return new TransformedResult
                            {
                                MasterID = hotelOffer.TeamVendorID.ToSafeInt(),
                                TPKey = hotelOffer.TeamVendorID,
                                CurrencyCode = "USD",
                                PropertyRoomBookingID = room.PRBID,
                                RoomType = hotelOffer.RoomType,
                                MealBasisCode = hotelOffer.MealPlan,
                                Amount = amount,
                                TPReference = $"{hotelOffer.ProductCode}|{hotelOffer.FamilyPlan}|{hotelOffer.ChildAge}",
                                NonRefundableRates = Equals(hotelOffer.NonRefundable.Trim(), "1")
                            };
                        });
                    })
                .ToList();

            var transformedResults = new TransformedResultCollection();
            transformedResults.TransformedResults.AddRange(results);

            return transformedResults;
        }

        #endregion

        #region SearchRestrictions

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            //'Must be between 1 to 21 nights
            bool moreThan21nights = searchDetails.Duration > 21;

            //'no more than 4 people in a room
            bool moreThan4peopleInAnyRoom = searchDetails.RoomDetails
                .Any(rd => (rd.Adults + rd.Children + rd.Infants) > 4);

            return moreThan21nights || moreThan4peopleInAnyRoom;
        }

        #endregion

        #region Helpers

        public string BuildRequestXml(SearchDetails serchDetails, ResortSplit resortSplit)
        {
            var request = new PriceSearch
            {
                UserName = _settings.Username(serchDetails),
                Password = _settings.Password(serchDetails),
                CityCode = resortSplit.ResortCode,
                ProductCode = "",
                RequestType = Constant.SearchTypeHotel,
                Occupancy = "",
                ArrivalDate = serchDetails.ArrivalDate.ToString(Constant.DateTimeFormat),
                NumberOfNights = serchDetails.Duration,
                NumberOfRooms = serchDetails.Rooms,
                DisplayClosedOut = Constant.TokenNo,
                DisplayOnRequest = Constant.TokenNo,
                VendorIds = resortSplit.Hotels.Select(oHotel => oHotel.TPKey).ToList()
            };

            var xmlDoc = Envelope<PriceSearch>.Serialize(request, _serializer);
            return xmlDoc;
        }

        #endregion
    }
}