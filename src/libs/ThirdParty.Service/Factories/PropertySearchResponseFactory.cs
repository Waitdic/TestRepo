namespace ThirdParty.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Intuitive;
    using iVector.Search.Property;
    using Microsoft.Extensions.Logging;
    using ThirdParty.Models;
    using ThirdParty.Models.Tokens;
    using ThirdParty.Repositories;
    using ThirdParty.SDK.V2.PropertySearch;
    using ThirdParty.Search.Models;
    using ThirdParty.Services;
    using ThirdParty.Utility;

    /// <summary>
    ///  Class responsible for building the property search response
    /// </summary>
    public class PropertySearchResponseFactory : IPropertySearchResponseFactory
    {
        /// <summary>The currency repository</summary>
        private readonly ICurrencyLookupRepository currencyRepository;

        /// <summary>The token service</summary>
        private ITokenService tokenService;

        /// <summary>The log writer</summary>
        private readonly ILogger<PropertySearchResponseFactory> _logger;

        /// <summary>Initializes a new instance of the <see cref="PropertySearchResponseFactory" /> class.</summary>
        /// <param name="currencyRepository">The currency repository.</param>
        /// <param name="tokenService">
        ///   <para>The token service, that encodes and decodes response and request tokens</para>
        /// </param>
        /// <param name="logger">The log writer</param>
        public PropertySearchResponseFactory(
            ICurrencyLookupRepository currencyRepository,
            ITokenService tokenService,
            ILogger<PropertySearchResponseFactory> logger)
        {
            this.currencyRepository = currencyRepository;
            this.tokenService = tokenService;
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

                var propertyID = supplierSplit.AllHotels.FirstOrDefault(h => h.TPKey == propertyData.TPKey).PropertyID;

                var currenyID = !string.IsNullOrEmpty(searchDetails.CurrencyCode) ?
                    (await currencyRepository.GetISOCurrencyIDFromISOCurrencyCodeAsync(searchDetails.CurrencyCode)) :
                    0;

                var propertyToken = new PropertyToken()
                {
                    ArrivalDate = searchDetails.ArrivalDate,
                    Duration = searchDetails.Duration,
                    PropertyID = propertyID,
                    Rooms = searchDetails.Rooms,
                    CurrencyID = currenyID
                };

                var propertyResult = new PropertyResult()
                {
                    BookingToken = this.tokenService.EncodePropertyToken(propertyToken),
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
                            LocalCost = PropertyFactoryHelper.splitNumberToNDigitList((int)(roomResult.PriceData.TotalCost * 100), 7),
                            MealBasis = PropertyFactoryHelper.splitNumberToNDigitList(roomResult.RoomData.MealBasisID, 2)
                        };

                        var roomType = new RoomType()
                        {
                            RoomID = roomResult.RoomData.PropertyRoomBookingID,
                            RoomBookingToken = this.tokenService.EncodeRoomToken(roomToken),
                            Supplier = roomResult.RoomData.Source,
                            MealBasisCode = roomResult.RoomData.MealBasisCode,
                            SupplierRoomType = roomResult.RoomData.RoomType,
                            SupplierReference1 = roomResult.RoomData.TPReference,
                            SupplierReference2 = roomResult.RoomData.RoomTypeCode,
                            Name = roomResult.RoomData.RoomType,
                            Code = roomResult.RoomData.RoomTypeCode,
                            CurrencyCode = await this.currencyRepository.GetCurrencyCodeFromISOCurrencyIDAsync(roomResult.RoomData.Source, roomResult.PriceData.CurrencyID),
                            TotalCost = Math.Round(roomResult.PriceData.TotalCost + 0.00M, 2),
                            NonRefundable = roomResult.PriceData.NonRefundableRates!.Value,
                            CancellationTerms = GetCancellationTerms(roomResult.Cancellations),
                            Discount = Math.Round(roomResult.PriceData.Discount + 0.00M, 2),
                            TPRateCode = roomResult.RoomData.RateCode,
                            Adjustments = GetAdjustments(roomResult.Adjustments),
                            CommissionPercentage = Math.Round(roomResult.PriceData.CommissionPercentage + 0.00M, 2)
                        };

                        propertyResult.RoomTypes.Add(roomType);
                    }
                    catch (Exception e)
                    {
                        this._logger.LogError(e, "BuildResultsError");
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
    }
}