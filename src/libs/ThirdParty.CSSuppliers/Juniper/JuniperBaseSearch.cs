namespace ThirdParty.CSSuppliers.Juniper
{
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using ThirdParty;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using ThirdParty.CSSuppliers.Juniper.Model;

    public abstract class JuniperBaseSearch : IThirdPartySearch
    {
        #region Properties

        private readonly IJuniperBaseSettings _settings;
        private readonly ISerializer _serializer;

        public abstract string Source { get; }

        #endregion

        #region Constructors

        public JuniperBaseSearch(IJuniperBaseSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Build Search Requests

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs)
        {
            var hotelList = new List<string>();
            int maxHotelsPerRequest = _settings.MaxHotelsPerSearchRequest(searchDetails);

            var searchRequests = searchDetails.RoomDetails.SelectMany((oRoom, roomCnt) =>
            {
                return resortSplits.SelectMany(resortSplit =>
                {
                    return resortSplit.Hotels
                        .Select((h, idx) => new { oHotel = h.TPKey, grIdx = idx / maxHotelsPerRequest })
                        .GroupBy(x => x.grIdx)
                        .Select(gr => new { hotels = gr.Select(x => x.oHotel), room = oRoom, prbid = roomCnt + 1 })
                        .ToList();
                });
            }).Take(Constant.SearchWebRequestLimit)
            .Select((x, iUniqueReqID) => BuildSearchWebRequest(searchDetails, x.hotels, saveLogs, x.room, x.prbid, iUniqueReqID))
            .ToList();

            return searchRequests;
        }

        public Request BuildSearchWebRequest(
            SearchDetails SearchDetails,
            IEnumerable<string> HotelList,
            bool SaveLogs,
            RoomDetail oRoom,
            int prbid,
            int iUniqueReqID)
        {
            string sSoapAction = _settings.HotelAvailURLSOAPAction(SearchDetails);
            bool useGZip = _settings.UseGZip(SearchDetails);
            string hotelAvailUrl = JuniperHelper.ConstructUrl(_settings.BaseURL(SearchDetails), _settings.HotelAvailURL(SearchDetails));

            var searchRequest = BuildSearchRequest(SearchDetails, HotelList.ToList(), oRoom);
            var sSearchRequest = JuniperHelper.BuildSoap(searchRequest, _serializer);

            var oSearchRequest = JuniperHelper.BuildWebRequest(hotelAvailUrl, sSoapAction, sSearchRequest, Constant.SearchLogFile, Source, SaveLogs, useGZip);

            var sUniqueCode = $"{Source}{iUniqueReqID}";
            var oExtraHelper = new SearchExtraHelper(SearchDetails, sUniqueCode);
            //' make a note of the room number so we know what we're dealing with in the transform
            oExtraHelper.ExtraInfo = prbid.ToString();

            oSearchRequest.ExtraInfo = oExtraHelper;

            return oSearchRequest;
        }

        private OTA_HotelAvailService BuildSearchRequest(SearchDetails oSearchDetails, List<string> hotelList, RoomDetail oRoom)
        {
            var hotelAvailRequest = new OTA_HotelAvailService
            {
                HotelAvailRequest = {
                    PrimaryLangId = _settings.LanguageCode(oSearchDetails),
                    Pos = JuniperHelper.BuildPosNode(_settings.AgentDutyCode(oSearchDetails), _settings.Password(oSearchDetails)),
                    AvailRequestSegmets = new()
                    {
                        new(){
                            StayDateRange = {
                                Start = oSearchDetails.ArrivalDate.ToString(Constants.DateTimeFormat),
                                End = oSearchDetails.DepartureDate.ToString(Constants.DateTimeFormat)
                            },
                            RoomStayCandidates = new()
                            {
                                new(){
                                    Quantity = 1,
                                    GuestCounts = new List<GuestCount>()
                                    {
                                        new GuestCount { Age = Constant.DefaultAdultAge, Count = oRoom.Adults },
                                        new GuestCount { Age = Constant.DefaultInfantAge, Count = oRoom.Infants }
                                    }
                                    .Concat(oRoom.ChildAges.GroupBy(ch => ch)
                                        .Select(ageGr => new GuestCount{ Age = ageGr.Key, Count = ageGr.Count()}))
                                    .Where(gc => gc.Count > 0)
                                    .ToList()
                                }
                            },
                            HotelSearchCriteria = hotelList.Select((sTPKey, i) =>
                            new Criterion
                            {
                                HotelRef = { HotelCode = sTPKey },
                                Extensions = i > 0 ? new(): new () {
                                    ShowCatalogueData = _settings.ShowCatalogueData(oSearchDetails)?"1":"0",
                                    ForceCurrency = _settings.CurrencyCode(oSearchDetails)??string.Empty,
                                    PaxCountry = _settings.PaxCountry(oSearchDetails)??string.Empty,
                                    ShowBasicInfo = "0",
                                    ShowPromotions = "0",
                                    ShowOnlyAvailable = "1"
                                }
                            }).ToList()
                        }
                    }
                }
            };

            return hotelAvailRequest;
        }

        #endregion

        #region Transform Response

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var excludeNonRefundableRates = _settings.ExcludeNonRefundableRates(searchDetails);

            foreach (var request in requests)
            {
                var oResponseXml = _serializer.CleanXmlNamespaces(request.ResponseXML);
                var sResponse = oResponseXml.SelectSingleNode("Envelope/Body").FirstChild.OuterXml;
                var oResponse = _serializer.DeSerialize<OTA_HotelAvailServiceResponse>(sResponse);
                var prbid = SafeTypeExtensions.ToSafeInt((request.ExtraInfo as SearchExtraHelper)?.ExtraInfo);

                if (oResponse.HotelAvailResponse.FirstOrDefault()?.Success ?? false)
                {
                    foreach (var hotelResponse in oResponse.HotelAvailResponse)
                    {
                        var SequenceNumber = hotelResponse.SequenceNmbr;
                        foreach (var roomStay in hotelResponse.RoomStays)
                        {
                            foreach (var roomRate in roomStay.RoomRates.Where(rr => rr.AvailabilityStatus == Constant.AvailableForSale))
                            {
                                var tr = new TransformedResult
                                {
                                    TPKey = roomStay.BasicPropertyInfo.HotelCode,
                                    CurrencyCode = roomStay.Total.CurrencyCode,
                                    PropertyRoomBookingID = prbid,
                                    RoomType = roomRate.Rates.First().RateDescription.Text,
                                    MealBasisCode = roomRate.Rates.First().RateExtension.MealPlan.Content,
                                    Adults = 0,
                                    Children = 0,
                                    ChildAgeCSV = string.Empty,
                                    Infants = 0,
                                    Amount = SafeTypeExtensions.ToSafeDecimal(roomRate.Rates.First().Total.AmountAfterTax),
                                    TPReference = $"{SequenceNumber}|{roomRate.RatePlanCode}|{roomStay.Total.CurrencyCode}",
                                    SpecialOffer = roomRate.Features.FirstOrDefault(f => string.Equals(f.RoomViewCode, Constant.RoomViewCodePromo)
                                                                            && string.Equals(f.Description.Name, Constant.SpecialOfferFeatureName))?.Description.Text ?? "",
                                    Discount = SafeTypeExtensions.ToSafeDecimal(roomRate.Features.FirstOrDefault(f => string.Equals(f.RoomViewCode, Constant.RoomViewCodePromo) &&
                                                                string.Equals(f.Description.Name, Constant.DiscountFeatureName))?.Description.Text ?? "0.00"),
                                    NonRefundableRates = string.Equals(roomRate.RoomRateExtension.NonRefundable, "1"),
                                    Adjustments = roomRate.RoomRateExtension.Elements.Select(e => new TransformedResultAdjustment
                                    {
                                        AdjustmentType = "S",
                                        AdjustmentName = e.Name,
                                        AdjustmentAmount = SafeTypeExtensions.ToSafeDecimal(e.Price),
                                        AdjustmentDescription = e.Description.Replace('{', '(').Replace('}', ')')
                                    }).ToList()
                                };
                                if (excludeNonRefundableRates == false || tr.NonRefundableRates == false) transformedResults.TransformedResults.Add(tr);
                            }
                        }
                    }
                }
            }

            return transformedResults;
        }

        #endregion

        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        public bool SearchRestrictions(SearchDetails oSearchDetails)
        {
            return oSearchDetails.Rooms > 1 && !SafeTypeExtensions.ToSafeBoolean(_settings.SplitMultiroom(oSearchDetails));
        }
    }
}
