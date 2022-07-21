namespace iVectorOne.Suppliers.DerbySoft
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using iVectorOne.Suppliers.DerbySoft.Models;

    public class DailyRateRetriever
    {
        public (List<decimal>, List<decimal>) GetDailyRates(
            RoomRate roomRate,
            int totalPax,
            DateTime arrivalDate,
            DateTime departureDate,
            out decimal feeAmount)
        {
            feeAmount = 0M;
            var dailyRates = (new List<decimal>(), new List<decimal>());
            if (roomRate.DailyCostAfterTax != null && roomRate.DailyCostAfterTax.Any())
            {
                dailyRates.Item2 = roomRate.DailyCostAfterTax.ToList();
            }

            if (roomRate.DailyCostBeforeTax == null || !roomRate.DailyCostBeforeTax.Any())
            {
                return dailyRates;
            }

            if (roomRate.Fees != null && roomRate.Fees.Count() > 0)
            {
                var feeCalculator = new FeeCalculator();
                feeAmount = feeCalculator.CalculateFees(
                    roomRate.Fees.ToList(),
                    totalPax,
                    new FeeCalculator.DateRange(arrivalDate, departureDate),
                    roomRate.DailyCostBeforeTax.ToList());
            }

            dailyRates.Item1 = roomRate.DailyCostBeforeTax.ToList();
            return dailyRates;
        }
    }
}