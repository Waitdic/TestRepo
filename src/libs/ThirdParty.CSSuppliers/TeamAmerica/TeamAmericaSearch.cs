namespace ThirdParty.CSSuppliers.TeamAmerica
{
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using ThirdParty;
    using ThirdParty.Constants;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using ThirdParty.CSSuppliers.TeamAmerica.Models;

    public class TeamAmericaSearch : IThirdPartySearch
    {
        #region "Properties"

        private readonly ITeamAmericaSettings _settings;
        private readonly ISerializer _serializer;

        public string Source => ThirdParties.TEAMAMERICA;

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion

        #region "Constructors"

        public TeamAmericaSearch(ITeamAmericaSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region "SearchFunctions"

        public List<Request> BuildSearchRequests(SearchDetails oSerchDetails, List<ResortSplit> oResortSplits, bool bSaveLogs)
        {
            var oRequests = oResortSplits.Select(oResortSplit =>
            {
                var oRequestXML = BuildRequestXml(oSerchDetails, oResortSplit);

                //'set a unique code. if the is one request we only need the source name
                string sUniqueCode = (oResortSplits.Count() > 1)
                                    ? $"{Source}_{oResortSplit.ResortCode}"
                                    : Source;

                var oRequest = new Request
                {
                    EndPoint = _settings.URL(oSerchDetails),
                    SoapAction = $"{_settings.URL(oSerchDetails)}/{Constant.SoapActionPreBook}",
                    Method = eRequestMethod.POST,
                    ContentType = ContentTypes.Text_Xml_charset_utf_8,
                    Source = Source,
                    LogFileName = Constant.LogFileSearch,
                    CreateLog = bSaveLogs,
                    ExtraInfo = new SearchExtraHelper(oSerchDetails, sUniqueCode),
                };
                oRequest.SetRequest(oRequestXML);

                return oRequest;
            }).ToList();
            return oRequests;
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var oResponses = requests.SelectMany(oRequest => Envelope<PriceSearchResponse>
                                        .DeSerialize(oRequest.ResponseXML, _serializer).HotelSearchResponse.Offers).ToList();

            var aRooms = searchDetails.RoomDetails.ToArray().Select((oRoomDetail, iRoomIndex) => new
            {
                PRBID = iRoomIndex + 1,
                Adults = oRoomDetail.Adults,
                Children = oRoomDetail.Children + oRoomDetail.Infants,
                ChildAges = oRoomDetail.ChildAndInfantAges()
            });

            var occupancyDict = new Dictionary<int, string>
            {
                { 1, "Single"},
                { 2, "Double"},
                { 3, "Triple"},
                { 4, "Quad"},
            };

            var aTransformedResults = oResponses.Where(oHotelOffer => TeamAmerica.IsEveryNightAvailable(oHotelOffer)
                                                                   && !string.IsNullOrEmpty(oHotelOffer.RoomType))
            .SelectMany(oHotelOffer =>
            {
                return aRooms.Where(oRoom => oRoom.Adults + oRoom.Children <= oHotelOffer.MaxOccupancy)
                .Select(oRoom =>
                {
                    int paxCount = string.Equals(oHotelOffer.FamilyPlan, Constant.TokenYes)
                    ? oRoom.Adults + oRoom.ChildAges.Where(age => age > oHotelOffer.ChildAge).Count()
                    : oRoom.Adults + oRoom.Children;

                    occupancyDict.TryGetValue(paxCount, out string occupancy);

                    decimal amount = oHotelOffer.NightlyInfos.Select(info => SafeTypeExtensions.ToSafeDecimal(
                        info.Prices.First(price => string.Equals(price.Occupancy, occupancy)).AdultPrice))
                        .Sum();

                    return new TransformedResult
                    {
                        MasterID = SafeTypeExtensions.ToSafeInt(oHotelOffer.TeamVendorID),
                        TPKey = oHotelOffer.TeamVendorID,
                        CurrencyCode = "USD",
                        PropertyRoomBookingID = oRoom.PRBID,
                        RoomType = oHotelOffer.RoomType,
                        MealBasisCode = oHotelOffer.MealPlan,
                        Amount = amount,
                        TPReference = $"{oHotelOffer.ProductCode}|{oHotelOffer.FamilyPlan}|{oHotelOffer.ChildAge}",
                        NonRefundableRates = Equals(oHotelOffer.NonRefundable.Trim(), "1")
                    };
                });
            }).ToList();

            var transformedResults = new TransformedResultCollection();
            transformedResults.TransformedResults.AddRange(aTransformedResults);

            return transformedResults;
        }

        #endregion

        #region "SearchRestrictions"

        public bool SearchRestrictions(SearchDetails searchDetails)
        {
            //'Must be between 1 to 21 nights
            bool moreThan21nights = searchDetails.Duration > 21;

            //'no more than 4 people in a room
            bool moreThan4peopleInAnyRoom = searchDetails.RoomDetails
                .Any(rd => (rd.Adults + rd.Children + rd.Infants) > 4);

            return moreThan21nights || moreThan4peopleInAnyRoom;
        }

        #endregion

        #region "Helpers"

        public string BuildRequestXml(SearchDetails oSerchDetails, ResortSplit oResortSplit)
        {
            var request = new PriceSearch
            {
                UserName = _settings.Username(oSerchDetails),
                Password = _settings.Password(oSerchDetails),
                CityCode = oResortSplit.ResortCode,
                ProductCode = "",
                RequestType = Constant.SearchTypeHotel,
                Occupancy = "",
                ArrivalDate = oSerchDetails.PropertyArrivalDate.ToString(Constant.DateTimeFormat),
                NumberOfNights = oSerchDetails.PropertyDuration,
                NumberOfRooms = oSerchDetails.Rooms,
                DisplayClosedOut = Constant.TokenNo,
                DisplayOnRequest = Constant.TokenNo,
                VendorIds = oResortSplit.Hotels.Select(oHotel => oHotel.TPKey).ToList()
            };

            var xmlDoc = Envelope<PriceSearch>.Serialize(request, _serializer);
            return xmlDoc;
        }

        #endregion
    }
}