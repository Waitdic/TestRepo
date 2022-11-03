namespace iVectorOne.Suppliers.Juniper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using Intuitive.Helpers.Net;
    using iVector.Search.Property;
    using iVectorOne.Suppliers.Juniper.Model;
    using iVectorOne.Interfaces;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;

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
            int iMaxHotelsPerRequest = _settings.HotelBatchLimit(searchDetails, source);

            var hotelRoomBatches = resortSplits.SelectMany(resortSplit => resortSplit.Hotels.Select(h => h.TPKey))
                .Batch(iMaxHotelsPerRequest)
                .SelectMany(tpKeys => searchDetails.RoomDetails.Select((oRoom, roomIdx) =>
                                new { hotels = tpKeys, room = oRoom, prbid = roomIdx + 1 }))
                .Take(Constant.SearchWebRequestLimit);

            var searchRequests = hotelRoomBatches.Select(x =>
                    BuildSearchWebRequest(searchDetails, source, x.hotels, x.room, x.prbid)).ToList();

            return Task.FromResult(searchRequests);
        }

        public Request BuildSearchWebRequest(
            SearchDetails SearchDetails,
            string source,
            IEnumerable<string> HotelList,
            RoomDetail oRoom,
            int prbid)
        {
            string soapAction = _settings.SOAPAvailableHotels(SearchDetails, source);
            bool useGZip = _settings.UseGZip(SearchDetails,source);
            string hotelAvailUrl = JuniperHelper.ConstructUrl(
                _settings.GenericURL(SearchDetails, source), 
                _settings.SearchURL(SearchDetails, source));

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
                        _settings.OperatorCode(searchDetails, source), 
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
                                    ForceCurrency = _settings.Currency(searchDetails, source)??string.Empty,
                                    PaxCountry = _settings.CustomerCountryCode(searchDetails, source)??string.Empty,
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
            var excludeNonRefundableRates = _settings.ExcludeNRF(searchDetails, source);

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
                                    Amount = roomRate.Rates.First().Total.AmountAfterTax.ToSafeDecimal(),
                                    TPReference = $"{SequenceNumber}|{roomRate.RatePlanCode}|{roomStay.Total.CurrencyCode}",
                                    SpecialOffer = roomRate.Features.FirstOrDefault(f => string.Equals(f.RoomViewCode, Constant.RoomViewCodePromo)
                                                                            && string.Equals(f.Description.Name, Constant.SpecialOfferFeatureName))?.Description.Text ?? "",
                                    Discount = (roomRate.Features.FirstOrDefault(f => string.Equals(f.RoomViewCode, Constant.RoomViewCodePromo) &&
                                                                string.Equals(f.Description.Name, Constant.DiscountFeatureName))?.Description.Text ?? "0.00").ToSafeDecimal(),
                                    NonRefundableRates = string.Equals(roomRate.RoomRateExtension.NonRefundable, "1"),
                                    Adjustments = roomRate.RoomRateExtension.Elements.Select(e => new TransformedResultAdjustment(SDK.V2.PropertySearch.AdjustmentType.Supplement, e.Name, 
                                                  e.Description.Replace('{', '(').Replace('}', ')'), e.Price.ToSafeDecimal())).ToList()
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