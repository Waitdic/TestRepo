namespace ThirdParty.CSSuppliers.ExpediaRapid
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Net.WebRequests;
    using MoreLinq;
    using Newtonsoft.Json;
    using ThirdParty.Constants;
    using ThirdParty.Lookups;
    using ThirdParty.Models;
    using ThirdParty.Results;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Support;
    using ThirdParty.CSSuppliers.ExpediaRapid.RequestConstants;
    using ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses;
    using ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Search;

    public class ExpediaRapidSearch : IThirdPartySearch
    {
        private readonly IExpediaRapidSettings _settings;

        private readonly ITPSupport _support;

        public ExpediaRapidSearch(IExpediaRapidSettings settings, ITPSupport support)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        public string Source { get; } = ThirdParties.EXPEDIARAPID;

        public static Request BuildDefaultRequest(string url, eRequestMethod requestMethod, RequestHeaders headers, bool useGzip, bool saveLogs, string userAgent, string logFileName, string requestBody = null)
        {
            var request = new Request()
            {
                Accept = "application/json",
                ContentType = "application/json",
                UseGZip = useGzip,
                AuthenticationMode = eAuthenticationMode.None,
                Method = requestMethod,
                Headers = headers,
                UserAgent = userAgent,
                EndPoint = url,
                CreateLog = saveLogs,
                Source = ThirdParties.EXPEDIARAPID,
                LogFileName = logFileName,
                SuppressExpectHeaders = true
            };

            if (!string.IsNullOrWhiteSpace(requestBody))
                request.SetRequest(requestBody);

            return request;
        }

        public Request BuildSearchRequest(IEnumerable<string> tpKeys, SearchDetails searchDetails, bool savelogs)
        {
            string searchURL = BuildSearchURL(tpKeys, searchDetails);
            bool useGzip = _settings.get_UseGZIP(searchDetails);
            string apiKey = _settings.get_ApiKey(searchDetails);
            string secret = _settings.get_Secret(searchDetails);
            string userAgent = _settings.get_UserAgent(searchDetails);

            string tpSessionID = Guid.NewGuid().ToString();
            var headers = new RequestHeaders() { new RequestHeader(SearchHeaderKeys.CustomerSessionID, tpSessionID), CreateAuthorizationHeader(apiKey, secret) };

            var request = BuildDefaultRequest(searchURL, eRequestMethod.GET, headers, useGzip, savelogs, userAgent, "Search");

            request.ExtraInfo = new SearchExtraHelper() { SearchDetails = searchDetails };

            return request;
        }

        public List<Request> BuildSearchRequests(SearchDetails searchDetails, List<ResortSplit> resortSplits, bool saveLogs)
        {
            int batchSize = _settings.get_SearchRequestBatchSize(searchDetails);
            var tpPropertyIDs = resortSplits.SelectMany(rs => rs.Hotels).Select(h => h.TPKey).ToList();

            return MoreLinq.Extensions.BatchExtension.Batch(tpPropertyIDs, batchSize).Select(tpKeys => BuildSearchRequest(tpKeys, searchDetails, saveLogs)).ToList();
        }

        public static string BuildSearchURL(IEnumerable<string> tpKeys, IExpediaRapidSettings settings, IThirdPartyAttributeSearch tpAttributeSearch, DateTime arrivalDate, DateTime departureDate, string currencyCode, IEnumerable<ExpediaRapidOccupancy> occupancies)
        {
            var nvc = new NameValueCollection() { { SearchQueryKeys.CheckIn, arrivalDate.ToString("yyyy-MM-dd") }, { SearchQueryKeys.CheckOut, departureDate.ToString("yyyy-MM-dd") }, { SearchQueryKeys.Currency, currencyCode }, { SearchQueryKeys.Language, settings.get_LanguageCode(tpAttributeSearch) }, { SearchQueryKeys.CountryCode, settings.get_CountryCode(tpAttributeSearch) }, { SearchQueryKeys.SalesChannel, settings.get_SalesChannel(tpAttributeSearch) }, { SearchQueryKeys.SalesEnvironment, settings.get_SalesEnvironment(tpAttributeSearch) }, { SearchQueryKeys.SortType, settings.get_SortType(tpAttributeSearch) }, { SearchQueryKeys.RatePlanCount, settings.get_RatePlanCount(tpAttributeSearch).ToString() }, { SearchQueryKeys.PaymentTerms, settings.get_PaymentTerms(tpAttributeSearch) }, { SearchQueryKeys.PartnerPointOfSale, settings.get_PartnerPointOfSale(tpAttributeSearch) }, { SearchQueryKeys.BillingTerms, settings.get_BillingTerms(tpAttributeSearch) }, { SearchQueryKeys.RateOption, settings.get_RateOption(tpAttributeSearch) } };

            if (!string.IsNullOrWhiteSpace(settings.get_PlatformName(tpAttributeSearch)))
            {
                nvc.Add(SearchQueryKeys.PlatformName, settings.get_PlatformName(tpAttributeSearch));
            }

            foreach (ExpediaRapidOccupancy occupancy in occupancies)
                nvc.Add(SearchQueryKeys.Occupancy, occupancy.GetExpediaRapidOccupancy());

            foreach (string tpKey in tpKeys)
                nvc.Add(SearchQueryKeys.PropertyID, tpKey);

            var searchUrl = new UriBuilder(settings.get_Scheme(tpAttributeSearch), settings.get_Host(tpAttributeSearch), -1, settings.get_SearchPath(tpAttributeSearch)); // to not put the port number in the URL

            return searchUrl.Uri.AbsoluteUri + AddQueryParams(nvc);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var allAvailabilities = new List<PropertyAvailablility>();

            string tpSessionID = requests.First().ResponseHeaders["Transaction-Id"];

            foreach (Request request in requests)
            {
                var response = new SearchResponse();
                bool success = response.IsValid(request.ResponseString, (int)request.ResponseStatusCode);

                if (success)
                {
                    response = JsonConvert.DeserializeObject<SearchResponse>(request.ResponseString);
                    allAvailabilities.AddRange(response);
                }
            }

            transformedResults.TransformedResults.AddRange(allAvailabilities.SelectMany(pa => GetResultFromPropertyAvailability(searchDetails, pa, tpSessionID)));

            return transformedResults;
        }

        public bool SearchRestrictions(SearchDetails searchDetails)
        {

            return searchDetails.Rooms > 8;
        }

        public bool ResponseHasExceptions(Request request)
        {
            var searchResponse = new SearchResponse();

            return !request.Success || !searchResponse.IsValid(request.ResponseString, (int)request.ResponseStatusCode);
        }

        internal static string AddQueryParams(NameValueCollection @params)
        {
            var queryString = from key in @params.AllKeys
                              from value in @params.GetValues(key)
                              select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));

            return "?" + string.Join("&", queryString);
        }

        private string BuildSearchURL(IEnumerable<string> tpKeys, SearchDetails searchDetails)
        {
            var arrivalDate = searchDetails.ArrivalDate;
            var departureDate = searchDetails.DepartureDate;

            string currencyCode = _support.TPCurrencyLookup(Source, searchDetails.CurrencyCode);

            var occupancies = searchDetails.RoomDetails.Select(r => new ExpediaRapidOccupancy(r.Adults, r.ChildAges, r.Infants));

            return BuildSearchURL(tpKeys, _settings, searchDetails, arrivalDate, departureDate, currencyCode, occupancies);
        }

        private IEnumerable<TransformedResult> GetResultFromPropertyAvailability(SearchDetails searchDetails, PropertyAvailablility propertyAvailability, string tpSessionID)
        {
            if (searchDetails.Rooms > 1)
            {
                var cheapestRoom = propertyAvailability.Rooms.OrderBy(room => room.Rates.Min(rate => GetTotalInclusiveRate(rate.OccupancyRoomRates))).First();


                propertyAvailability.Rooms.RemoveAll(r => (r.RoomID ?? "") != (cheapestRoom.RoomID ?? ""));
            }

            return propertyAvailability.Rooms.SelectMany(room => BuildResultFromRoom(searchDetails, propertyAvailability.PropertyID, room, tpSessionID));
        }

        private decimal GetTotalInclusiveRate(Dictionary<string, OccupancyRoomRate> occupancyRoomRates)
        {
            return occupancyRoomRates.Sum(orr => orr.Value.OccupancyRateTotals["inclusive"].TotalInRequestCurrency.Amount);
        }

        private IEnumerable<TransformedResult> BuildResultFromBedGroups(SearchDetails searchDetails, string tpKey, SearchResponseRoom room, RoomRate rate, BedGroupAvailability bedGroup, string tpSessionID)
        {

            var occupancies = searchDetails.RoomDetails.Select(r => new ExpediaRapidOccupancy(r.Adults, r.ChildAges, r.Infants));

            return occupancies.Select((occupancy, i) => BuildResultFromOccupancy(tpKey, room, rate, bedGroup, occupancy, i + 1, tpSessionID));
        }

        private TransformedResult BuildResultFromOccupancy(string tpKey, SearchResponseRoom room, RoomRate rate, BedGroupAvailability bedGroup, ExpediaRapidOccupancy occupancy, int propertyRoomBookingID, string tpSessionID)
        {
            var occupancyRoomRate = rate.OccupancyRoomRates[occupancy.GetExpediaRapidOccupancy()];

            var inclusiveRate = occupancyRoomRate.OccupancyRateTotals["inclusive"].TotalInRequestCurrency;

            decimal baseRate = GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.BaseRate);

            var taxes = GetTaxes(occupancyRoomRate);

            var adjustments = GetAdjustments(taxes);

            decimal totalTax = taxes.Sum(t => t.TaxAmount);

            var lookupMealBasisCodes = Enumerable.ToHashSet(_support.TPMealBases(Source).Keys);

            string mealBasisCode = GetMealBasisCode(rate.Amenities.Keys, lookupMealBasisCodes);

            var specialOffers = GetSpecialOffers(rate, lookupMealBasisCodes);

            var result = new TransformedResult();

            if (occupancyRoomRate is null)
            {
                result.Warnings.Add("No room rate found for this property.");
            }
            else
            {
                result.MasterID = tpKey.ToSafeInt();
                result.TPKey = tpKey;
                result.CurrencyCode = inclusiveRate.CurrencyCode;
                result.PropertyRoomBookingID = propertyRoomBookingID;
                result.RoomType = $"{room.RoomName}, {bedGroup.Description}";
                result.RoomTypeCode = $"{room.RoomID}|{rate.RateID}|{bedGroup.BedGroupID}";
                result.MealBasisCode = mealBasisCode;
                result.Adults = occupancy.Adults;
                result.ChildAges = occupancy.ChildAges;
                result.Children = occupancy.ChildAges.Where(i => i > 0).Count();
                result.Infants = occupancy.ChildAges.Where(i => i == 0).Count();
                result.Amount = inclusiveRate.Amount;
                result.RegionalTax = totalTax.ToString();
                result.SpecialOffer = string.Join(", ", specialOffers);
                result.Discount = baseRate + totalTax - inclusiveRate.Amount;
                result.NonRefundableRates = !rate.IsRefundable;
                result.AvailableRooms = rate.AvailableRooms;
                result.Adjustments = adjustments;
                result.PayLocalRequired = rate.MerchantOfRecord == "property";
                result.TPReference = $"{tpSessionID}|{bedGroup.Links.PriceCheckLink.HRef}";
            }

            result.Validate();

            return result;
        }

        private IEnumerable<TransformedResult> BuildResultFromRoom(SearchDetails searchDetails, string tpKey, SearchResponseRoom room, string tpSessionID)
        {
            if (searchDetails.Rooms > 1)
            {
                var cheapestRate = room.Rates.MinBy(rate => GetTotalInclusiveRate(rate.OccupancyRoomRates)).First();


                room.Rates.RemoveAll(r => (r.RateID ?? "") != (cheapestRate.RateID ?? ""));
            }

            return room.Rates.SelectMany(rate => BuildResultFromRoomRates(searchDetails, tpKey, room, rate, tpSessionID));
        }

        private IEnumerable<TransformedResult> BuildResultFromRoomRates(SearchDetails searchDetails, string tpKey, SearchResponseRoom room, RoomRate rate, string tpSessionID)
        {
            return rate.BedGroupAvailabilities.Values.SelectMany(bedGroup => BuildResultFromBedGroups(searchDetails, tpKey, room, rate, bedGroup, tpSessionID));
        }

        private List<TransformedResultAdjustment> GetAdjustments(List<Tax> taxes)
        {
            return taxes.Select(t => new TransformedResultAdjustment()
            {
                AdjustmentType = "T",
                PayLocal = true,
                AdjustmentName = t.TaxName,
                AdjustmentAmount = t.TaxAmount
            }).ToList();
        }

        private List<Tax> GetTaxes(OccupancyRoomRate occupancyRoomRate)
        {
            decimal salesTax = GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.SalesTax);
            decimal taxAndServiceFee = GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.TaxAndServiceFee);
            decimal recoverChargesAndFees = GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.RecoveryChargesAndFees);

            if (occupancyRoomRate.StayRates.Any())
            {
                salesTax += GetStayRateFromType(occupancyRoomRate, RateTypes.SalesTax);
                taxAndServiceFee += GetStayRateFromType(occupancyRoomRate, RateTypes.TaxAndServiceFee);
                recoverChargesAndFees += GetStayRateFromType(occupancyRoomRate, RateTypes.RecoveryChargesAndFees);
            }

            var taxes = new List<Tax>();
            if (salesTax > 0m)
                taxes.Add(new Tax("Sales Tax", salesTax));
            if (taxAndServiceFee > 0m)
                taxes.Add(new Tax("Tax And Service Fee", taxAndServiceFee));
            if (recoverChargesAndFees > 0m)
                taxes.Add(new Tax("Tax Recovery Charges", recoverChargesAndFees));

            return taxes;
        }

        public static decimal GetTotalNightlyRateFromType(OccupancyRoomRate occupancyRoomRate, string rateType)
        {
            return occupancyRoomRate.NightlyRates.SelectMany(nr => nr.Where(amt => (amt.RateType ?? "") == (rateType ?? ""))).Sum(amt => amt.Amount);
        }

        public static decimal GetStayRateFromType(OccupancyRoomRate occupancyRoomRate, string rateType)
        {
            var stayRates = occupancyRoomRate.StayRates;

            var matchedRate = stayRates.Where(r => (r.RateType ?? "") == (rateType ?? ""));

            return matchedRate.Any() ? matchedRate.First().Amount : 0m;
        }

        private List<string> GetSpecialOffers(RoomRate rate, HashSet<string> lookupMealBasisCodes)
        {
            var specialOffers = new List<string>();

            IEnumerable<string> nonMealBasisAmenities = rate.Amenities.Where(a => !lookupMealBasisCodes.Contains(a.Key)).Select(a => a.Value.Name).ToList();
            specialOffers.AddRange(nonMealBasisAmenities);

            if (rate.Promotions is not null)
            {

                if (rate.Promotions.ValueAdds is not null)
                {
                    IEnumerable<string> valueAdds = rate.Promotions.ValueAdds.Select(vaKvp => vaKvp.Value.Description).ToList();

                    specialOffers.AddRange(valueAdds);
                }

                if (rate.Promotions.Deal is not null)
                {
                    specialOffers.Add(rate.Promotions.Deal.Description);
                }

            }

            return specialOffers;
        }

        internal string GetMealBasisCode(IEnumerable<string> amenities, HashSet<string> tpValidMealbasisCodes)
        {
            var matchedMealBasisCodes = amenities.Where(a => tpValidMealbasisCodes.Contains(a));

            return matchedMealBasisCodes.Any() ? matchedMealBasisCodes.First() : "RO";
        }

        public static RequestHeader CreateAuthorizationHeader(string apiKey, string secret)
        {
            double timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var data = Encoding.UTF8.GetBytes($"{apiKey}{secret}{timeStamp}");

            string hashString;

            using (SHA512 sha = new SHA512Managed())
            {
                hashString = BitConverter.ToString(sha.ComputeHash(data)).Replace("-", "").ToLower();
            }

            return new RequestHeader(SearchHeaderKeys.Authorization, $"EAN apikey={apiKey},signature={hashString},timestamp={timeStamp}");
        }

        private class Tax
        {
            public Tax(string name, decimal amount)
            {
                TaxName = name;
                TaxAmount = amount;
            }
            public string TaxName { get; set; }
            public decimal TaxAmount { get; set; }
        }

        private class QueryParamter
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}