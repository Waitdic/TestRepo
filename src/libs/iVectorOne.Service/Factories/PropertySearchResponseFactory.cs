namespace iVectorOne.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVector.Search.Property;
    using Microsoft.Extensions.Logging;
    using iVectorOne.Lookups;
    using iVectorOne.Models;
    using iVectorOne.Models.Tokens;
    using iVectorOne.Repositories;
    using iVectorOne.SDK.V2.PropertySearch;
    using iVectorOne.Search.Models;
    using iVectorOne.Services;
    using iVectorOne.Utility;

    /// <summary>
    ///  Class responsible for building the property search response
    /// </summary>
    public class PropertySearchResponseFactory : IPropertySearchResponseFactory
    {
        /// <summary>The token service</summary>
        private readonly ITPSupport _support;

        /// <summary>The token service</summary>
        private readonly ITokenService _tokenService;

        /// <summary>The log writer</summary>
        private readonly ILogger<PropertySearchResponseFactory> _logger;

        /// <summary>Initializes a new instance of the <see cref="PropertySearchResponseFactory" /> class.</summary>
        /// <param name="support">The third party support</param>
        /// <param name="tokenService">The token service, that encodes and decodes response and request tokens</param>
        /// <param name="logger">The log writer</param>
        public PropertySearchResponseFactory(
            ITPSupport support,
            ITokenService tokenService,
            ILogger<PropertySearchResponseFactory> logger)
        {
            _support = Ensure.IsNotNull(support, nameof(support));
            _tokenService = Ensure.IsNotNull(tokenService, nameof(tokenService));
            _logger = Ensure.IsNotNull(logger, nameof(logger));
        }

        /// <summary>Creates the specified results.</summary>
        /// <param name="searchDetails">The search details, used to retrieve information about the search e.g. duration that is not on the results</param>
        /// <param name="supplierSplits">The resort splits, contains information looked up at start of search</param>
        /// <param name="requestTracker">The request tracker, allows for analysis of reponse times</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<Response> CreateAsync(SearchDetails searchDetails, List<SupplierResortSplit> supplierSplits, IRequestTracker requestTracker)
        {
            var response = new Response();
            var createResponseTimer = new ThirdPartyRequestTime("CreateResponse");
            createResponseTimer.StartTotalTimer();

            foreach (var result in searchDetails.Results.DedupeResults)
            {
                var propertyData = result.Value.PropertyData;

                var supplierSplit = supplierSplits.FirstOrDefault(ss => ss.Supplier == propertyData.Source);

                var propertyId = supplierSplit.AllHotels.FirstOrDefault(h => h.TPKey == propertyData.TPKey).PropertyID;

                var currencyId = result.Value.RoomResults.First().PriceData.CurrencyID;

                var propertyToken = new PropertyToken()
                {
                    ArrivalDate = searchDetails.ArrivalDate,
                    Duration = searchDetails.Duration,
                    PropertyID = propertyId,
                    Rooms = searchDetails.Rooms,
                    CurrencyID = currencyId
                };

                var propertyResult = new PropertyResult()
                {
                    BookingToken = _tokenService.EncodePropertyToken(propertyToken),
                    PropertyID = propertyData.PropertyReferenceID
                };

                foreach (var roomResult in result.Value.RoomResults)
                {
                    try
                    {
                        var roomToken = new RoomToken()
                        {
                            Adults = searchDetails.RoomDetails[roomResult.RoomData.PropertyRoomBookingID - 1].Adults,
                            Children = searchDetails.RoomDetails[roomResult.RoomData.PropertyRoomBookingID - 1].Children,
                            Infants = searchDetails.RoomDetails[roomResult.RoomData.PropertyRoomBookingID - 1].Infants,
                            ChildAges = searchDetails.RoomDetails[roomResult.RoomData.PropertyRoomBookingID - 1].ChildAges,
                            PropertyRoomBookingID = roomResult.RoomData.PropertyRoomBookingID,
                            LocalCost = PropertyFactoryHelper.SplitNumberToNDigitList((int)(roomResult.PriceData.TotalCost * 100), 7),
                            MealBasisID = PropertyFactoryHelper.SplitNumberToNDigitList(roomResult.RoomData.MealBasisID, 2)
                        };

                        var roomType = new RoomType()
                        {
                            RoomID = roomResult.RoomData.PropertyRoomBookingID,
                            RoomBookingToken = _tokenService.EncodeRoomToken(roomToken),
                            Supplier = roomResult.RoomData.Source,
                            MealBasisCode = roomResult.RoomData.MealBasisCode,
                            SupplierRoomType = roomResult.RoomData.RoomType,
                            SupplierReference1 = roomResult.RoomData.TPReference,
                            SupplierReference2 = roomResult.RoomData.RoomTypeCode,
                            Name = roomResult.RoomData.RoomType,
                            Code = roomResult.RoomData.RoomTypeCode,
                            CurrencyCode = await _support.ISOCurrencyCodeLookupAsync(roomResult.PriceData.CurrencyID),
                            TotalCost = Math.Round(roomResult.PriceData.TotalCost + 0.00M, 2),
                            NonRefundable = roomResult.PriceData.NonRefundableRates!.Value,
                            CancellationTerms = GetCancellationTerms(roomResult.Cancellations),
                            Discount = Math.Round(roomResult.PriceData.Discount + 0.00M, 2),
                            RateCode = roomResult.RoomData.RateCode,
                            Adjustments = GetAdjustments(roomResult.Adjustments),
                            CommissionPercentage = Math.Round(roomResult.PriceData.CommissionPercentage + 0.00M, 2),
                            OnRequest = roomResult.RoomData.OnRequest,
                            GrossCost = GetGrossCost(roomResult),
                            SellingPrice = roomResult.PriceData.SellingPrice,
                            RateBasis = roomResult.PriceData.RateBasis
                        };

                        propertyResult.RoomTypes.Add(roomType);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "BuildResultsError");
                    }
                }

                response.PropertyResults.Add(propertyResult);
            }

            createResponseTimer.StopTotalTimer();
            createResponseTimer.SetTimes();
            requestTracker.RequestTimes.Add(createResponseTimer);

            return response;
        }

        private List<CancellationTerm> GetCancellationTerms(List<Cancellation> cancellations)
        {
            var cancellationTerms = new List<CancellationTerm>();
            foreach (var cancellation in cancellations)
            {
                cancellationTerms.Add(new CancellationTerm()
                {
                    StartDate = cancellation.StartDate,
                    EndDate = cancellation.EndDate,
                    Amount = Math.Round(cancellation.Amount + 0.00M, 2)
                });
            }

            return cancellationTerms;
        }

        private List<SDK.V2.PropertySearch.Adjustment> GetAdjustments(List<iVector.Search.Property.Adjustment> adjustments)
        {
            return adjustments.Select(x => new SDK.V2.PropertySearch.Adjustment(Enum.Parse<AdjustmentType>(x.AdjustmentType), x.AdjustmentName,
                x.CustomerNotes, x.TotalCost)).ToList();
        }

        /// <summary>
        /// Gets the gross cost. If equal to total cost then default is returned. 
        /// Default values are ignored and not displayed in the response
        /// </summary>
        /// <param name="roomResult">The room search result</param>
        /// <returns>The gross cost</returns>
        private decimal GetGrossCost(RoomSearchResult roomResult)
        {
            return roomResult.PriceData.GrossCost == roomResult.PriceData.TotalCost
                                        ? default(decimal) : Math.Round(roomResult.PriceData.GrossCost + 0.00M, 2);
        }
    }
}