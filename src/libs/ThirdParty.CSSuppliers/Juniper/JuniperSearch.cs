namespace ThirdParty.CSSuppliers.Juniper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Net.WebRequests;
    using iVector.Search.Property;
    using ThirdParty;
    using ThirdParty.CSSuppliers.Juniper.Model;
    using ThirdParty.Interfaces;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;

    public class JuniperSearch : IThirdPartySearch, IMultiSource
    {
        #region Properties

        private readonly IJuniperSettings _settings;
        private readonly ISerializer _serializer;

        public List<string> Sources => Constant.JuniperSources;

        #endregion

        #region Constructors

        public JuniperSearch(IJuniperSettings settings, ISerializer serializer)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        #endregion

        #region Build Search Requests

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            string source = resortSplits.First().ThirdPartySupplier;
            var hotelList = new List<string>();
            int maxHotelsPerRequest = _settings.MaxHotelsPerSearchRequest(searchDetails, source);

            var searchRequests = searchDetails.RoomDetails
                .SelectMany((oRoom, roomCnt) =>
                    {
                        return resortSplits.SelectMany(resortSplit =>
                        {
                            return resortSplit.Hotels
                                .Select((h, idx) => new { oHotel = h.TPKey, grIdx = idx / maxHotelsPerRequest })
                                .GroupBy(x => x.grIdx)
                                .Select(gr => new { hotels = gr.Select(x => x.oHotel), room = oRoom, prbid = roomCnt + 1 })
                                .ToList();
                        });
                    })
                .Take(Constant.SearchWebRequestLimit)
                .Select((x, iUniqueReqID) => BuildSearchWebRequest(searchDetails, source, x.hotels, x.room, x.prbid, iUniqueReqID))
                .ToList();

            return Task.FromResult(searchRequests);
        }

        public Request BuildSearchWebRequest(
            SearchDetails SearchDetails,
            string source,
            IEnumerable<string> HotelList,
            RoomDetail oRoom,
            int prbid,
            int iUniqueReqID)
        {
            string soapAction = _settings.HotelAvailURLSOAPAction(SearchDetails, source);
            bool useGZip = _settings.UseGZip(SearchDetails, source);
            string hotelAvailUrl = JuniperHelper.ConstructUrl(
                _settings.BaseURL(SearchDetails, source),
                _settings.HotelAvailURL(SearchDetails, source));

            var searchRequestBody = BuildSearchRequest(SearchDetails, source, HotelList.ToList(), oRoom);
            var searchRequestSoap = JuniperHelper.BuildSoap(searchRequestBody, _serializer);

            var searchRequest = JuniperHelper.BuildWebRequest(hotelAvailUrl, soapAction, searchRequestSoap, Constant.SearchLogFile, source, useGZip);

            searchRequest.ExtraInfo = prbid;

            return searchRequest;
        }

        private OTA_HotelAvailService BuildSearchRequest(
            SearchDetails searchDetails,
            string source,
            List<string> hotelList,
            RoomDetail room)
        {
            var hotelAvailRequest = new OTA_HotelAvailService
            {
                HotelAvailRequest = {
                    PrimaryLangId = _settings.LanguageCode(searchDetails, source),
                    Pos = JuniperHelper.BuildPosNode(
                        _settings.AgentDutyCode(searchDetails, source),
                        _settings.Password(searchDetails, source)),
                    AvailRequestSegmets = new()
                    {
                        new(){
                            StayDateRange = {
                                Start = searchDetails.ArrivalDate.ToString(Constants.DateTimeFormat),
                                End = searchDetails.DepartureDate.ToString(Constants.DateTimeFormat)
                            },
                            RoomStayCandidates = new()
                            {
                                new(){
                                    Quantity = 1,
                                    GuestCounts = new List<GuestCount>()
                                    {
                                        new GuestCount { Age = Constant.DefaultAdultAge, Count = room.Adults },
                                        new GuestCount { Age = Constant.DefaultInfantAge, Count = room.Infants }
                                    }
                                    .Concat(room.ChildAges.GroupBy(ch => ch)
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
                                    ShowCatalogueData = _settings.ShowCatalogueData(searchDetails, source)?"1":"0",
                                    ForceCurrency = _settings.CurrencyCode(searchDetails, source)??string.Empty,
                                    PaxCountry = _settings.PaxCountry(searchDetails, source)??string.Empty,
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
            string source = resortSplits.First().ThirdPartySupplier;
            var transformedResults = new TransformedResultCollection();
            var excludeNonRefundableRates = _settings.ExcludeNonRefundableRates(searchDetails, source);

            foreach (var request in requests)
            {
                var oResponseXml = _serializer.CleanXmlNamespaces(request.ResponseXML);
                var sResponse = oResponseXml.SelectSingleNode("Envelope/Body").FirstChild.OuterXml;
                var oResponse = _serializer.DeSerialize<OTA_HotelAvailServiceResponse>(sResponse);
                var prbid = request.ExtraInfo.ToSafeInt();

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

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return searchDetails.Rooms > 1 && !_settings.SplitMultiroom(searchDetails, source);
        }
    }
}