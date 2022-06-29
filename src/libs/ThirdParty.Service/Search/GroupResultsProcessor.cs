namespace ThirdParty.Search
{
    using System; 
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive.Helpers.Extensions;
    using iVector.Search.Property;
    using ThirdParty.Repositories;
    using ThirdParty.SDK.V2.PropertySearch;
    using ThirdParty.Search.Models;
    using ThirdParty.Search.Results.Models;

    /// <summary>
    /// Groups third Party results by property into our own format
    /// </summary>
    public class GroupResultsProcessor : IGroupResultsProcessor
    {
        /// <summary>The filter</summary>
        private readonly IFilter _filter;

        /// <summary>The currency look up repository</summary>
        private readonly ICurrencyLookupRepository _currencyRepository;

        /// <summary>Repository for retrieving third party meal basis</summary>
        private readonly IMealBasisLookupRepository _mealbasisRepository;

        /// <summary>Initializes a new instance of the <see cref="GroupResultsProcessor" /> class.</summary>
        /// <param name="filter">The property filter</param>
        /// <param name="currencyRepository">The currency look up repository</param>
        /// <param name="mealbasisRepository">Repository for retrieving third party meal basis</param>
        public GroupResultsProcessor(IFilter filter, ICurrencyLookupRepository currencyRepository, IMealBasisLookupRepository mealbasisRepository)
        {
            _filter = filter;
            _currencyRepository = currencyRepository;
            _mealbasisRepository = mealbasisRepository;
        }

        /// <summary>Groups the property results.</summary>
        /// <param name="thirdPartyResults">The third party results.</param>
        /// <param name="source">The source.</param>
        /// <param name="searchDetails">The search details.</param>
        /// <param name="resortSplits">the resort splits</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<List<PropertySearchResult>> GroupPropertyResultsAsync(TransformedResultCollection thirdPartyResults, string source, SearchDetails searchDetails, IEnumerable<IResortSplit> resortSplits)
        {
            var results = new List<PropertySearchResult>();
            foreach (var searchResult in thirdPartyResults.DistinctValidResults)
            {
                var result = results.FirstOrDefault(x => x.PropertyData.TPKey == searchResult.TPKey);

                if (result == null)
                {
                    result = new PropertySearchResult()
                    {
                        PropertyData = new PropertyData()
                        {
                            Source = source,
                            TPKey = searchResult.TPKey,
                        }
                    };
                    results.Add(result);
                }

                var roomResult = new RoomSearchResult()
                {
                    RoomData = new RoomData()
                    {
                        PropertyRoomBookingID = GetPropertyRoomBookingID(searchResult, searchDetails.RoomDetails),
                        TPReference = searchResult.TPReference,
                        RoomType = searchResult.RoomType,
                        RoomTypeCode = searchResult.RoomTypeCode,
                        Adults = searchResult.Adults,
                        Children = searchResult.Children,
                        Youths = searchResult.Infants,
                        Source = source,
                        NonRefundable = searchResult.NonRefundableRates ?? false,
                        PayLocalRequired = searchResult.PayLocalRequired,
                        PayLocalAvailable = searchResult.PayLocalAvailable,
                        MealBasisCode = await _mealbasisRepository.GetMealBasisfromTPMealbasisCodeAsync(source, searchResult.MealBasisCode),
                        MealBasisID = await _mealbasisRepository.GetMealBasisIDfromTPMealbasisCodeAsync(source, searchResult.MealBasisCode),
                        RateCode  = searchResult.TPRateCode
                    },
                    PriceData = new PriceData()
                    {
                        TotalCost = searchResult.Amount.ToSafeDecimal(),
                        Total = searchResult.Amount.ToSafeDecimal(),
                        NonRefundableRates = searchResult.NonRefundableRates,
                        Discount = searchResult.Discount
                    },

                    Cancellations = searchResult.Cancellations.Select(x => new Cancellation()
                    {
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Amount = x.Amount,
                    }).ToList(),

                    Adjustments = searchResult.Adjustments.Select(x => new iVector.Search.Property.Adjustment() 
                    {
                        AdjustmentType = Enum.GetName(typeof(AdjustmentType),x.AdjustmentType),
                        AdjustmentName = x.AdjustmentName,
                        TotalCost = x.AdjustmentAmount,
                        CustomerNotes = x.AdjustmentDescription
                    }).ToList()
                };

                await SetCurrencyID(roomResult, result.PropertyData, searchResult);

                if (roomResult.RoomData.MealBasisCode != string.Empty && roomResult.RoomData.PropertyRoomBookingID != 0)
                {
                    result.RoomResults.Add(roomResult);
                }
            }

            results = _filter.ProcessResults(results, resortSplits, searchDetails);

            return results;
        }

        private async Task SetCurrencyID(RoomSearchResult roomResult, PropertyData propertyData, TransformedResult searchResult)
        {
            int currencyId = searchResult.CurrencyID;

            if (!string.IsNullOrWhiteSpace(searchResult.CurrencyCode))
            {
                currencyId = await _currencyRepository.GetISOCurrencyIDFromSupplierCurrencyCodeAsync(propertyData.Source, searchResult.CurrencyCode);
            }

            roomResult.PriceData.CurrencyID = currencyId;
        }

        /// <summary>Gets the property room booking identifier.</summary>
        /// <param name="searchResult">The search result.</param>
        /// <param name="roomDetails">The room details.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private int GetPropertyRoomBookingID(TransformedResult searchResult, RoomDetails roomDetails)
        {
            int propertyRoomBookingId = searchResult.PropertyRoomBookingID;

            if (propertyRoomBookingId == 0)
            {
                // some TPs don't specifiy a room ID so take the first one matching the pax
                var matchingRoom = roomDetails.FirstOrDefault(x => x.Adults == searchResult.Adults &&
                                                                x.Children == searchResult.Children &&
                                                                x.Infants == searchResult.Infants);
                if (matchingRoom != null)
                {
                    propertyRoomBookingId = matchingRoom.PropertyRoomBookingID;
                }
            }

            return propertyRoomBookingId;
        }
    }
}
