namespace iVectorOne.Search
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Models;
    using iVectorOne.Repositories;
    using iVectorOne.Search.Models;

    /// <summary>
    ///   <br />
    /// </summary>
    public class ResultsDeduper : IResultDeduper
    {
        private readonly ICurrencyLookupRepository _currencyRepository;

        public ResultsDeduper(ICurrencyLookupRepository currencyRepository)
        {
            _currencyRepository = Ensure.IsNotNull(currencyRepository, nameof(currencyRepository));
        }

        /// <summary>Groups the properties asynchronous.</summary>
        /// <param name="results">The results.</param>
        /// <param name="searchDetails"> a search details</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<int> DedupeResultsAsync(List<iVector.Search.Property.PropertySearchResult> results, SearchDetails searchDetails)
        {
            var propertyList = GroupProperties(results, searchDetails);

            int totalProperties = propertyList.Keys.Select(o => o.Split('_')[0]).Distinct().Count();

            foreach (string key in propertyList.Keys)
            {
                var propertyResult = propertyList[key];
                int checkCentralPropertyID = propertyResult.PropertyData.PropertyReferenceID;
                int checkPropertyID = key.Split('_')[0].ToSafeInt();
                int checkMealBasisID = key.Split('_')[1].ToSafeInt();
                bool checkNonRefundable = key.Split('_')[2].ToSafeBoolean();

                // get the sum of the cheapest lead in prices across all property room bookings
                var dedupeResult = new iVector.Search.Property.DedupeSearchResult(propertyResult);
                int currencyID = dedupeResult.RoomResults.OrderBy(r => r.PriceData.Total).First().PriceData.CurrencyID;
                decimal exchangeRate = await _currencyRepository.GetExchangeRateFromISOCurrencyIDAsync(currencyID);
                decimal checkLeadInPrice = dedupeResult.LeadInPrice * exchangeRate;

                if (!searchDetails.DedupeResults)
                {
                    if (checkCentralPropertyID > 0)
                    {
                        // any unique key will do but the first split "_" needs to be property reference id
                        int keyCount = searchDetails.ConcurrentResults.Keys.Count;
                        string checkHashCode = $"{checkCentralPropertyID}_{checkMealBasisID}_{(checkNonRefundable ? 1 : 0)}_{keyCount}";

                        // todo - loop until this is added
                        searchDetails.ConcurrentResults.TryAdd(checkHashCode, dedupeResult);
                    }
                }
                else
                {
                    string checkHashCode = $"{checkCentralPropertyID}_{checkMealBasisID}_{(checkNonRefundable ? 1 : 0)}";
                    if (checkCentralPropertyID > 0)
                    {
                        searchDetails.ConcurrentResults.AddOrUpdate(
                            checkHashCode,
                            dedupeResult,
                            (key, currentProperty) =>
                            {
                                int currentCurrencyID = currentProperty.RoomResults.OrderBy(r => r.PriceData.Total).First().PriceData.CurrencyID;
                                decimal currentExchangeRate = _currencyRepository.GetExchangeRateFromISOCurrencyIDAsync(currentCurrencyID).Result;
                                decimal currentLeadinPrice = currentProperty.LeadInPrice * currentExchangeRate;

                                // if we have cheaper lead in price, or the same price either a lower priority number or lower propertyid
                                if (currentLeadinPrice > checkLeadInPrice)
                                {
                                    return dedupeResult;
                                }
                                else
                                {
                                    return currentProperty;
                                }
                            });
                    }
                }
            }

            return totalProperties;
        }

        /// <summary>Groups the properties.</summary>
        /// <param name="results">The results.</param>
        /// <param name="searchDetails">The search details.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private Dictionary<string, iVector.Search.Property.PropertySearchResult> GroupProperties(List<iVector.Search.Property.PropertySearchResult> results, SearchDetails searchDetails)
        {
            var dedupeMethod = searchDetails.Settings.SingleRoomDedupingAlgorithm;
            bool dedupeByNonRefundable = searchDetails.Settings.DedupeByNonRefundable;
            bool unknownNonRefundableAsRefundable = searchDetails.Settings.UnknownNonRefundableAsRefundable;

            var propertyList = new Dictionary<string, iVector.Search.Property.PropertySearchResult>();

            // Get a property and rooms for each combination of property and mealbasis and nonrefundable
            foreach (var result in results)
            {
                int propertyId = result.PropertyData.PropertyID;
                foreach (var room in result.RoomResults)
                {
                    string mealBasis = !searchDetails.DedupeResults || dedupeMethod == DedupeMethod.CheapestLeadin ? string.Empty : room.RoomData.MealBasisCode;

                    int nonRefundable = 0;
                    if (searchDetails.DedupeResults && dedupeByNonRefundable)
                    {
                        if (room.PriceData.NonRefundableRates.HasValue)
                        {
                            if (room.PriceData.NonRefundableRates.Value)
                            {
                                nonRefundable = 1;
                            }
                        }
                        else
                        {
                            nonRefundable = -1;
                        }
                    }

                    string hashKey = $"{propertyId}_{mealBasis}_{nonRefundable}";

                    if (!propertyList.TryGetValue(hashKey, out var property))
                    {
                        property = new iVector.Search.Property.PropertySearchResult()
                        {
                            PropertyData = result.PropertyData
                        };

                        propertyList.Add(hashKey, property);
                    }

                    room.PriceData.NonRefundableRates = room.PriceData.NonRefundableRates ?? !unknownNonRefundableAsRefundable;

                    property.RoomResults.Add(room);
                }
            }

            return propertyList;
        }
    }
}