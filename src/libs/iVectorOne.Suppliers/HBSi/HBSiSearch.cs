namespace iVectorOne.Suppliers.HBSi
{
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using Intuitive.Helpers.Serialization;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Suppliers.HBSi.Models;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class HBSiSearch : IThirdPartySearch, IMultiSource
    {
        #region "Properties"

        public List<string> Sources => Constant.HBSiSources;

        private readonly IHBSiSettings _settings;
        private readonly ITPSupport _support;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HBSi> _logger;

        protected bool GetMealBasisCodeFromRatePlanCode => false;

        #endregion

        #region "Constructors"
        public HBSiSearch(IHBSiSettings settings, ITPSupport support, ISerializer serializer, HttpClient httpClient, ILogger<HBSi> logger)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
            _httpClient = Ensure.IsNotNull(httpClient, nameof(httpClient));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        #endregion

        #region "Search Restrictions"

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return false;
        }

        #endregion

        #region "Search Functions"

        private string GetSource(List<ResortSplit> resortSplits) => resortSplits.First().ThirdPartySupplier;

        public Task<List<Request>> BuildSearchRequestsAsync(SearchDetails oSearchDetails, List<ResortSplit> oResortSplits)
        {
            var source = GetSource(oResortSplits);
            var oRequests = new List<Request>();
            //'Get a list of all hotel codes
            var oHotelCodes = oResortSplits.SelectMany(oResortSplit => oResortSplit.Hotels.Select(o => o.TPKey.Split('|')[0])).ToList();

            if (oHotelCodes.Any())
            {
                var dTimeStamp = HBSiHelper.Now();
                var oAvailRequest = BuildRequest(oSearchDetails, oHotelCodes, dTimeStamp, source);
                var xmlRequest = Envelope<OtaHotelAvailRq>.Serialize(_serializer, oAvailRequest, _settings, oSearchDetails, oHotelCodes[0],
                                                       "HotelAvailRQ", dTimeStamp.ToString("yyyyMMddhhmmssfff"), source);

                var oRequest = new Request
                {
                    EndPoint = _settings.GenericURL(oSearchDetails, source),
                    Method = RequestMethod.POST,
                    LogFileName = "Search",
                    TimeoutInSeconds = _settings.RequestTimeOutSeconds(oSearchDetails, source),
                    UseGZip = _settings.UseGZip(oSearchDetails, source)
                };

                oRequest.SetRequest(xmlRequest);

                oRequests.Add(oRequest);
            }

            return Task.FromResult(oRequests);
        }

        public TransformedResultCollection TransformResponse(List<Request> oRequests, SearchDetails oSearchDetails, List<ResortSplit> resortSplits)
        {
            var source = GetSource(resortSplits);
            var oSearchResponseXML = oRequests[0].ResponseXML;
            var oSearchResponse = Envelope<OtaHotelAvailRs>.DeSerialize(_serializer, oSearchResponseXML);

            bool firstRoomOnly = _settings.MRFirstRoomOnly(oSearchDetails, source);

            var oRooms = oSearchDetails.RoomDetails.Select((oRoomDetail, iRoomIdx) => new
            {
                iRoomIdx,
                oRoomDetail.Adults,
                oRoomDetail.Children,
                oRoomDetail.Infants,
                oGuestCounts = HBSiHelper.GetGuestCounts(_settings.UsePassengerAge(oSearchDetails, source), oRoomDetail)
            });

            var oRoomsTransformed = oSearchResponse.RoomStays.Select((oRoomStay, iRoomStayIdx) =>
            {
                return new
                {
                    oRoomStay,
                    iRoomStayIdx,
                    iTotalOccupancy = oRoomStay.GuestCounts.Sum(gs => gs.Count)
                };
            }).Where(x =>
            {
                return !(x.iRoomStayIdx > 0 && firstRoomOnly);
            }).SelectMany(x =>
            {
                bool NonRefundableRoom = x.oRoomStay.CancelPenalties.Any(
                    c => c.NonRefundable ||
                    string.Equals(c.Deadline.AbsoluteDeadline.Split('T')[0], HBSiHelper.Now().ToString(Constant.DateFormat))
                    && c.AmountPercent.Percent.ToSafeInt() == 100);

                string sHotelCode = x.oRoomStay.BasicPropertyInfo.HotelCode;
                decimal Amount = x.oRoomStay.Total.AmountAfterTax.ToSafeDecimal();


                return x.oRoomStay.RoomRates.Select((oRoomRate, iRoomRateIdx) =>
                {
                    string sRoomTypeCode = oRoomRate.RoomTypeCode;
                    string sRatePlanCode = oRoomRate.RatePlanCode;
                    string sRatePlanCodePrefix = sRatePlanCode.Split('_')[0];
                    string TPKey = $"{sHotelCode}|{_settings.PartnerName(oSearchDetails, source)}";


                    string[] HotelCodes = _settings.HotelCodesWithAdditionalRoomTypeInfo(oSearchDetails, source).Split(',');
                    string[] ExtraRoomTypes = _settings.AdditionalRoomTypeInfoValues(oSearchDetails, source).Split(',');

                    string sRatePlanType = x.oRoomStay.RatePlans.FirstOrDefault(rp => string.Equals(rp.RatePlanCode, sRatePlanCode))?.RatePlanType ?? "";

                    string RatePlanDescription = x.oRoomStay.RatePlans.FirstOrDefault(rp =>
                            string.Equals(rp.RatePlanCode, sRatePlanCode))?.RatePlanDescription.Name ?? "";

                    string sRoomType = HotelCodes.Contains(TPKey)
                        ? ExtraRoomTypes.FirstOrDefault(rt => RatePlanDescription.Contains(rt))
                        : x.oRoomStay.RoomTypes.FirstOrDefault(rt => string.Equals(rt.RoomTypeCode, sRoomTypeCode))?.RoomDescription.Name ?? "";


                    string sMealBasisCode = x.oRoomStay.RatePlans
                                .FirstOrDefault(rp => string.Equals(rp.RatePlanCode, sRatePlanCode))?.MealsIncluded.MealPlanCodes ?? "";


                    if (GetMealBasisCodeFromRatePlanCode && sRatePlanCode.Split('_').Count() > 2)
                    {
                        sMealBasisCode = sRatePlanCode.Split('_')[2];
                    }
                    else if (_settings.MealFromDescription(oSearchDetails, source))
                    {
                        string roomDescription = x.oRoomStay.RoomTypes.FirstOrDefault(rt => string.Equals(rt.RoomTypeCode, sRoomTypeCode))?.RoomDescription.Text ?? "";
                        sMealBasisCode = roomDescription.ToLower().Contains("breakfast")
                            ? "BB"
                            : "NOM";
                    }

                    string sMealBasisReferenceCode = _settings.MealFromDescription(oSearchDetails, source)
                        ? "NOM"
                        : sMealBasisCode;


                    int NumberOfUnits = oRoomRate.NumberOfUnits;
                    decimal AmountPerRoom = Amount / NumberOfUnits;
                    string RateCurrencyCode = oRoomRate.Rates.First().Total.CurrencyCode;

                    return new
                    {
                        sHotelCode,
                        oRoomRate,
                        iRoomRateIdx,
                        x.oRoomStay,
                        sRoomType,
                        sRoomTypeCode,
                        sMealBasisCode,
                        sMealBasisReferenceCode,
                        sRatePlanCode,
                        sRatePlanType,
                        sRatePlanCodePrefix,
                        AmountPerRoom,
                        NonRefundableRoom,
                        RateCurrencyCode,
                        TPKey
                    };
                }).Where(oRate =>
                {
                    // first room only restriction
                    bool firstRoomOnlyRestriction = !(oRate.iRoomRateIdx > 0 && firstRoomOnly);

                    // hotelRoomType occupancy restriction
                    string sMaxRoomOccupancy = _settings.MaxRoomOccupancy(oSearchDetails, source);
                    int iRoomCapacity = HBSiHelper.GetRoomCapacity(sMaxRoomOccupancy, oRate.sHotelCode, oRate.sRoomTypeCode);
                    bool occupancyRestriction = iRoomCapacity == 0 || iRoomCapacity >= x.iTotalOccupancy;

                    // There are 2 scenarios for a ratecode to pass
                    // 1) If it is not a package code: It's in the allowed filter or we do not have an allowed filter
                    // 2) If it is a package code (prefixed 11_): We want to return opaque rates or we are searching package rates
                    bool bAllowedRate = string.IsNullOrEmpty(_settings.RatePlanCodes(oSearchDetails, source))
                                      || _settings.RatePlanCodes(oSearchDetails, source).Split(',').Contains(oRate.sRatePlanCode);

                    bool SearchPackagePrices = oSearchDetails.OpaqueSearch;
                    bool IsPackageFlag = string.Equals(oRate.sRatePlanCodePrefix, _settings.PackageRateCode(oSearchDetails, source));
                    bool ReturnOpaqueRates = _settings.ReturnOpaqueRates(oSearchDetails, source);

                    bool packageModeRestriction = !IsPackageFlag && bAllowedRate || IsPackageFlag && (ReturnOpaqueRates || SearchPackagePrices);

                    return firstRoomOnlyRestriction
                        && occupancyRestriction
                        && packageModeRestriction;
                });
            }).SelectMany(oRate =>
            {
                //match room guest set
                return oRooms.Where(oRoom => HBSiHelper.GuestSetEquals(oRate.oRoomStay.GuestCounts, oRoom.oGuestCounts))
                             .Select(oRoom =>
                             {
                                 string TPReference = $"{oRate.sRoomTypeCode}|{oRate.sRatePlanCode}|{oRate.sMealBasisReferenceCode}"
                                                      + $"|{oRoom.Adults}|{oRoom.Children}|{oRoom.Infants}|{oRate.RateCurrencyCode}";

                                 return new TransformedResult
                                 {
                                     CurrencyCode = oRate.oRoomRate.Rates.First().Total.CurrencyCode,
                                     PropertyRoomBookingID = oRoom.iRoomIdx + 1,
                                     RoomType = oRate.sRoomType,
                                     MealBasisCode = oRate.sMealBasisCode,
                                     Amount = oRate.AmountPerRoom,
                                     TPKey = oRate.TPKey,
                                     NonRefundableRates = oRate.NonRefundableRoom,
                                     RateCode = oRate.sRatePlanCode,
                                     TPReference = TPReference
                                 };
                             });
            })
            .ToList();

            var resultCollection = new TransformedResultCollection();
            resultCollection.TransformedResults.AddRange(oRoomsTransformed);

            return resultCollection;
        }

        #endregion

        #region "Response Has Exceptions"
        public bool ResponseHasExceptions(Request request)
        {
            return false;
        }

        #endregion

        #region "Helpers"

        private OtaHotelAvailRq BuildRequest(SearchDetails oSearchDetails, List<string> oHotelCodes, DateTime dTimeStamp, string source)
        {
            var oAvailRq = new OtaHotelAvailRq
            {
                Target = _settings.Target(oSearchDetails, source),
                Version = _settings.Version(oSearchDetails, source),
                BestOnly = "false",
                SummaryOnly = "false",
                TimeStamp = dTimeStamp.ToString(Constant.TimeStampFormat),
                Pos = HBSiHelper.GetPos(oSearchDetails, _settings, source),
                AvailRequestSegmets =
                {
                    new AvailRequestSegment
                    {
                        HotelSearchCriteria =
                        {
                            new Criterion
                            {
                                StayDateRange =
                                {
                                    Start = oSearchDetails.ArrivalDate.ToString(Constant.DateFormat),
                                    End = oSearchDetails.DepartureDate.ToString(Constant.DateFormat)
                                },
                                RatePlanCandidates =
                                {
                                    new RatePlanCandidate
                                    {
                                        RatePlanCode = "*",
                                        RPH = "1",
                                        HotelRefs = oHotelCodes.Select(sHotelCode => new HotelRef
                                        {
                                            HotelCode = sHotelCode
                                        }).ToList(),
                                        MealsIncluded =
                                        {
                                            MealPlanCodes = "*"
                                        }
                                    }
                                },
                                //'if we have multiple room details then only add the ones with distinct guest counts
                                RoomStayCandidates = HBSiHelper.GetRoomStayCandidates(oSearchDetails, _settings, source)
                            }
                        }
                    }
                }
            };
            return oAvailRq;
        }

        #endregion
    }
}
