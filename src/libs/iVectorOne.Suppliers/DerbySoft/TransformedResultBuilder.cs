﻿namespace iVectorOne.Suppliers.DerbySoft
{
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using Intuitive;
    using iVectorOne.Suppliers.DerbySoft.Models;
    using iVectorOne.Models.Property.Booking;
    using iVectorOne.Search.Results.Models;
    using iVectorOne.Search.Models;

    public class TransformedResultBuilder
    {
        private readonly IDerbySoftSettings _settings;
        private readonly string _source;

        public TransformedResultBuilder(IDerbySoftSettings settings, string source)
        {
            _settings = Ensure.IsNotNull(settings, nameof(settings));
            _source = source;
        }

        public TransformedResult? BuildTransformedResult(
            SearchDetails searchDetails, 
            int propertyRoomBookingId,
            string tpKey,
            string headerToken,
            RoomRate roomRate)
        {
            var nonRefundableRates = roomRate.CancelPolicy != null && roomRate.CancelPolicy.Code == "AD100P_100P";

            if (_settings.ExcludeNRF(searchDetails, _source) && nonRefundableRates)
            {
                return null;
            }

            var dailyRateRetriever = new DailyRateRetriever();
            var dailyRates = dailyRateRetriever.GetDailyRates(
                                roomRate,
                                searchDetails.TotalPassengers,
                                searchDetails.ArrivalDate,
                                searchDetails.DepartureDate,
                                out var feeAmount);

            // set to amount after tax if both of the prices where returned or after tax was returned else to before tax
            var rates = dailyRates.Item2 != null && dailyRates.Item2.Count > 0 ? dailyRates.Item2 : dailyRates.Item1;

            var amount = rates.Sum() + feeAmount;

            if (amount == 0)
            {
                return null;
            }

            var cancellationCalculator = new CancellationCalculator();
            var cancellations =
                cancellationCalculator.GetCancellations(roomRate.CancelPolicy, rates, searchDetails.ArrivalDate);

            var transformedResult = new TransformedResult
            {
                RoomTypeCode = roomRate.RoomId,
                MealBasisCode = roomRate.MealPlan,
                Amount = amount,
                SellingPrice = amount,
                NetPrice = amount,
                CurrencyCode = roomRate.Currency,
                RateCode = roomRate.RateId,
                TPKey = tpKey,
                PropertyRoomBookingID = propertyRoomBookingId,
                NonRefundableRates = nonRefundableRates,
                Cancellations = cancellations.Select(c => new Cancellation()
                {
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Amount = c.Amount
                }).ToList(),
                TPReference =
                    PreBookHelper.SerializePreBookHelper(new PreBookHelper(headerToken, roomRate,
                        cancellations))
            };
            return transformedResult;
        }
    }
}