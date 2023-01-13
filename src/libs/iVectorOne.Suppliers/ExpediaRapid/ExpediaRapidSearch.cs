namespace iVectorOne.Suppliers.ExpediaRapid
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using System.Web;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Net;
    using MoreLinq;
    using iVectorOne.Constants;
    using iVectorOne.Suppliers.ExpediaRapid.RequestConstants;
    using iVectorOne.Suppliers.ExpediaRapid.SerializableClasses;
    using iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Search;
    using iVectorOne.Interfaces;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Models.Property;

    public class ExpediaRapidSearch : IThirdPartySearch, ISingleSource
    {
        private readonly IExpediaRapidSettings _settings;

        private readonly ITPSupport _support;

        public ExpediaRapidSearch(IExpediaRapidSettings settings, ITPSupport support)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _support = Ensure.IsNotNull(support, nameof(support));
        }

        public string Source => ThirdParties.EXPEDIARAPID;

        public async Task<List<Request>> BuildSearchRequestsAsync(SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            int batchSize = _settings.HotelBatchLimit(searchDetails);
            var tpPropertyIDs = resortSplits.SelectMany(rs => rs.Hotels).Select(h => h.TPKey).ToList();

            return (await Task.WhenAll(MoreLinq.Extensions.BatchExtension.Batch(tpPropertyIDs, batchSize)
                    .Select(async tpKeys => await BuildSearchRequestAsync(tpKeys, searchDetails))))
                .ToList();
        }

        private async Task<Request> BuildSearchRequestAsync(IEnumerable<string> tpKeys, SearchDetails searchDetails)
        {
            string searchURL = await BuildSearchURLAsync(tpKeys, searchDetails);
            bool useGzip = _settings.UseGZip(searchDetails);
            string apiKey = _settings.APIKey(searchDetails);
            string secret = _settings.Secret(searchDetails);
            string userAgent = _settings.UserAgent(searchDetails);

            string tpSessionID = Guid.NewGuid().ToString();
            var headers = new RequestHeaders()
            {
                new RequestHeader(SearchHeaderKeys.CustomerSessionID, tpSessionID),
                CreateAuthorizationHeader(apiKey, secret)
            };

            var request = BuildDefaultRequest(
                searchURL,
                RequestMethod.GET,
                headers,
                useGzip,
                userAgent,
                extraInfo: await _support.TPMealBasesAsync(Source));

            return request;
        }

        private async Task<string> BuildSearchURLAsync(IEnumerable<string> tpKeys, SearchDetails searchDetails)
        {
            var arrivalDate = searchDetails.ArrivalDate;
            var departureDate = searchDetails.DepartureDate;
            var countryCode = _settings.SourceMarket(searchDetails);

            if (searchDetails.SellingCountry != string.Empty)
            {
                countryCode = await _support.TPCountryCodeLookupAsync(Source, searchDetails.SellingCountry, searchDetails.Account.AccountID);
            }

            string currencyCode = await _support.TPCurrencyCodeLookupAsync(Source, searchDetails.ISOCurrencyCode);

            var occupancies = searchDetails.RoomDetails.Select(r => new ExpediaRapidOccupancy(r.Adults, r.ChildAges, r.Infants));

            return BuildSearchURL(tpKeys, _settings, searchDetails, arrivalDate, departureDate, currencyCode, countryCode, occupancies);
        }

        public static RequestHeader CreateAuthorizationHeader(string apiKey, string secret)
        {
            double timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var data = System.Text.Encoding.UTF8.GetBytes($"{apiKey}{secret}{timeStamp}");

            string hashString;

            using (SHA512 sha = new SHA512Managed())
            {
                hashString = BitConverter.ToString(sha.ComputeHash(data)).Replace("-", "").ToLower();
            }

            return new RequestHeader(SearchHeaderKeys.Authorization, $"EAN apikey={apiKey},signature={hashString},timestamp={timeStamp}");
        }

        public static Request BuildDefaultRequest(
            string url,
            RequestMethod requestMethod,
            RequestHeaders headers,
            bool useGzip,
            string userAgent,
            string requestBody = "",
            string logFileName = "",
            object extraInfo = null!)
        {
            var request = new Request()
            {
                Accept = "application/json",
                ContentType = "application/json",
                UseGZip = useGzip,
                AuthenticationMode = AuthenticationMode.None,
                Method = requestMethod,
                Headers = headers,
                UserAgent = userAgent,
                EndPoint = url,
                SuppressExpectHeaders = true,
                ExtraInfo = extraInfo,
                Source = ThirdParties.EXPEDIARAPID,
                LogFileName = logFileName
            };

            if (!string.IsNullOrWhiteSpace(requestBody))
            {
                request.SetRequest(requestBody);
            }

            return request;
        }

        public static string BuildSearchURL(
            IEnumerable<string> tpKeys,
            IExpediaRapidSettings settings,
            IThirdPartyAttributeSearch tpAttributeSearch,
            DateTime arrivalDate,
            DateTime departureDate,
            string currencyCode,
            string countryCode,
            IEnumerable<ExpediaRapidOccupancy> occupancies)
        {
            var nvc = new NameValueCollection()
            {
                { SearchQueryKeys.CheckIn, arrivalDate.ToString("yyyy-MM-dd") },
                { SearchQueryKeys.CheckOut, departureDate.ToString("yyyy-MM-dd") },
                { SearchQueryKeys.Currency, currencyCode },
                { SearchQueryKeys.Language, settings.LanguageCode(tpAttributeSearch) },
                { SearchQueryKeys.CountryCode, countryCode },
                { SearchQueryKeys.SalesChannel, settings.SalesChannel(tpAttributeSearch) },
                { SearchQueryKeys.SalesEnvironment, settings.SalesEnvironment(tpAttributeSearch) },
                { SearchQueryKeys.SortType, settings.SortType(tpAttributeSearch) }, //needed for 2.3 do not remove
                { SearchQueryKeys.RatePlanCount, settings.RatePlanCount(tpAttributeSearch).ToString() },
                { SearchQueryKeys.PaymentTerms, settings.PaymentTerms(tpAttributeSearch) },
                { SearchQueryKeys.PartnerPointOfSale, settings.PartnerPointOfSale(tpAttributeSearch) },
                { SearchQueryKeys.BillingTerms, settings.BillingTerms(tpAttributeSearch) },
                { SearchQueryKeys.RateOption, settings.RateOption(tpAttributeSearch) }
            };

            if (!string.IsNullOrWhiteSpace(settings.PlatformName(tpAttributeSearch)))
            {
                nvc.Add(SearchQueryKeys.PlatformName, settings.PlatformName(tpAttributeSearch));
            }

            foreach (var occupancy in occupancies)
            {
                nvc.Add(SearchQueryKeys.Occupancy, occupancy.GetExpediaRapidOccupancy());
            }

            foreach (string tpKey in tpKeys)
            {
                nvc.Add(SearchQueryKeys.PropertyID, tpKey);
            }

            var searchUrl = new UriBuilder(settings.Scheme(tpAttributeSearch), settings.Host(tpAttributeSearch), -1, settings.SearchURL(tpAttributeSearch)); // to not put the port number in the URL

            return searchUrl.Uri.AbsoluteUri + AddQueryParams(nvc);
        }

        private static string AddQueryParams(NameValueCollection @params)
        {
            // todo - move to utilities as a query string builder / serializer
            var queryString = from key in @params.AllKeys
                              from value in @params.GetValues(key)
                              select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));

            return "?" + string.Join("&", queryString);
        }

        public TransformedResultCollection TransformResponse(List<Request> requests, SearchDetails searchDetails, List<ResortSplit> resortSplits)
        {
            var transformedResults = new TransformedResultCollection();
            var allAvailabilities = new List<PropertyAvailablility>();
            var mealbases = (Dictionary<string, int>)requests.First().ExtraInfo!;

            string tpSessionID = requests.First().ResponseHeaders["Transaction-Id"];

            foreach (var request in requests)
            {
                var response = new SearchResponse();
                (bool valid, response) = response.GetValidResults(request.ResponseString, (int)request.ResponseStatusCode);
                allAvailabilities.AddRange(response);
            }

            transformedResults.TransformedResults.AddRange(allAvailabilities.SelectMany(pa => GetResultFromPropertyAvailability(searchDetails, pa, tpSessionID, mealbases)));

            return transformedResults;
        }

        private IEnumerable<TransformedResult> GetResultFromPropertyAvailability(
            SearchDetails searchDetails,
            PropertyAvailablility propertyAvailability,
            string tpSessionID,
            Dictionary<string, int> mealBases)
        {
            if (searchDetails.Rooms > 1)
            {
                var cheapestRoom = propertyAvailability.Rooms.OrderBy(room => room.Rates.Min(rate => GetTotalInclusiveRate(rate.OccupancyRoomRates))).First();

                propertyAvailability.Rooms.RemoveAll(r => (r.RoomID ?? "") != (cheapestRoom.RoomID ?? ""));
            }

            return propertyAvailability.Rooms.SelectMany(room => BuildResultFromRoom(searchDetails, propertyAvailability.PropertyID, room, tpSessionID, mealBases));
        }

        private decimal GetTotalInclusiveRate(Dictionary<string, OccupancyRoomRate> occupancyRoomRates)
        {
            return occupancyRoomRates.Sum(orr => orr.Value.OccupancyRateTotals["inclusive"].TotalInRequestCurrency.Amount);
        }

        private IEnumerable<TransformedResult> BuildResultFromRoom(
            SearchDetails searchDetails,
            string tpKey,
            SearchResponseRoom room,
            string tpSessionID,
            Dictionary<string, int> mealBases)
        {
            if (searchDetails.Rooms > 1)
            {
                var cheapestRate = room.Rates.MinBy(rate => GetTotalInclusiveRate(rate.OccupancyRoomRates)).First();

                room.Rates.RemoveAll(r => (r.RateID ?? "") != (cheapestRate.RateID ?? ""));
            }

            return room.Rates.SelectMany(rate => BuildResultFromRoomRates(searchDetails, tpKey, room, rate, tpSessionID, mealBases));
        }

        private IEnumerable<TransformedResult> BuildResultFromRoomRates(
            SearchDetails searchDetails,
            string tpKey,
            SearchResponseRoom room,
            RoomRate rate,
            string tpSessionID,
            Dictionary<string, int> mealBases)
        {
            return rate.BedGroupAvailabilities.Values.SelectMany(bedGroup => BuildResultFromBedGroups(searchDetails, tpKey, room, rate, bedGroup, tpSessionID, mealBases));
        }

        private IEnumerable<TransformedResult> BuildResultFromBedGroups(
            SearchDetails searchDetails,
            string tpKey,
            SearchResponseRoom room,
            RoomRate rate,
            BedGroupAvailability bedGroup,
            string tpSessionID,
            Dictionary<string, int> mealBases)
        {
            var occupancies = searchDetails.RoomDetails.Select(r => new ExpediaRapidOccupancy(r.Adults, r.ChildAges, r.Infants));

            return occupancies.Select((occupancy, i) => BuildResultFromOccupancy(searchDetails, tpKey, room, rate, bedGroup, occupancy, i + 1, tpSessionID, mealBases));
        }

        private TransformedResult BuildResultFromOccupancy(
            SearchDetails searchDetails,
            string tpKey,
            SearchResponseRoom room,
            RoomRate rate,
            BedGroupAvailability bedGroup,
            ExpediaRapidOccupancy occupancy,
            int propertyRoomBookingID,
            string tpSessionID,
            Dictionary<string, int> mealBases)
        {
            var occupancyRoomRate = rate.OccupancyRoomRates[occupancy.GetExpediaRapidOccupancy()];

            var inclusiveRate = occupancyRoomRate.OccupancyRateTotals["inclusive"].TotalInRequestCurrency;

            OccupancyRateAmount? marketingFee = null;
            OccupancyRateAmount? billableInclusiveRate = null;

            if (occupancyRoomRate.OccupancyRateTotals.ContainsKey("inclusive")
                && occupancyRoomRate.OccupancyRateTotals.ContainsKey("marketing_fee")) 
            {
                marketingFee = occupancyRoomRate.OccupancyRateTotals["marketing_fee"].TotalInBillableCurrency;
                billableInclusiveRate = occupancyRoomRate.OccupancyRateTotals["inclusive"].TotalInBillableCurrency;
            }

            decimal baseRate = GetTotalNightlyRateFromType(occupancyRoomRate, RateTypes.BaseRate);

            var taxes = GetTaxes(occupancyRoomRate);

            var adjustments = GetAdjustments(taxes);

            decimal totalTax = taxes.Sum(t => t.TaxAmount);

            var lookupMealBasisCodes = Enumerable.ToHashSet(mealBases.Keys);

            string mealBasisCode = GetMealBasisCode(rate.Amenities.Keys, lookupMealBasisCodes);

            var specialOffers = GetSpecialOffers(rate, lookupMealBasisCodes);

            var cancellations = GetCancellations(searchDetails, rate, occupancyRoomRate);

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
                result.RegionalTax = totalTax.ToString();
                result.SpecialOffer = string.Join(", ", specialOffers);
                result.Discount = baseRate + totalTax - inclusiveRate.Amount;
                result.NonRefundableRates = !rate.IsRefundable;
                result.AvailableRooms = rate.AvailableRooms;
                result.Adjustments = adjustments;
                result.PayLocalRequired = rate.MerchantOfRecord == "property";
                result.TPReference = $"{tpSessionID}|{bedGroup.Links.PriceCheckLink.HRef}";
                result.Cancellations = cancellations;

                if (marketingFee != null && marketingFee.Amount > 0)
                {
                    result.CommissionPercentage = marketingFee.Amount / billableInclusiveRate.Amount * 100;
                    result.Amount = billableInclusiveRate.Amount;
                    result.PackageRateBasis = "Gross Margin";
                }
                else 
                {
                    result.Amount = inclusiveRate.Amount;
                    result.PackageRateBasis = "Net";
                }
            }

            result.Validate();

            return result;
        }

        private List<TransformedResultAdjustment> GetAdjustments(List<Tax> taxes)
        {
            return taxes.Select(t => new TransformedResultAdjustment(SDK.V2.PropertySearch.AdjustmentType.Tax, t.TaxName, "", t.TaxAmount)).ToList();
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

        public bool SearchRestrictions(SearchDetails searchDetails, string source)
        {
            return searchDetails.Rooms > 8;
        }

        public bool ResponseHasExceptions(Request request)
        {
            return !request.Success;
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

        private Cancellations GetCancellations(SearchDetails searchDetails, RoomRate roomRate, OccupancyRoomRate occupancyRoomRate)
        {
            var cancellations = new Cancellations();

            if (!roomRate.IsRefundable && !roomRate.CancelPenalities.Any())
            {
                decimal roomAmount = occupancyRoomRate.OccupancyRateTotals["inclusive"].TotalInRequestCurrency.Amount;
                cancellations.Add(new Cancellation(DateTime.Now.Date, searchDetails.ArrivalDate, roomAmount));
            }

            cancellations.AddRange(roomRate.CancelPenalities.Select(cp => BuildCancellation(occupancyRoomRate, cp)));

            return cancellations;
        }

        private Cancellation BuildCancellation(OccupancyRoomRate occupancyRoomRate, CancelPenalty cancelPenalty)
        {
            decimal amount = 0m;

            if (cancelPenalty.Amount != 0m)
            {
                amount += cancelPenalty.Amount;
            }

            if (!string.IsNullOrEmpty(cancelPenalty.Percent))
            {
                decimal percent = cancelPenalty.Percent.Replace("%", "").ToSafeDecimal();
                decimal roomAmount = occupancyRoomRate.OccupancyRateTotals["inclusive"].TotalInRequestCurrency.Amount;
                amount += Math.Round(roomAmount / 100m * percent, 2, MidpointRounding.AwayFromZero);
            }

            if (cancelPenalty.Nights > 0)
            {
                var nightCancellations = occupancyRoomRate.NightlyRates.Take(cancelPenalty.Nights).ToList();
                foreach (List<Rate> night in nightCancellations)
                {
                    foreach (Rate rate in night)
                        amount += rate.Amount;
                }
            }

            var cancellation = new Cancellation(cancelPenalty.CancelStartDate, cancelPenalty.CancelEndDate, amount);

            return cancellation;
        }
    }
}