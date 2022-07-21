namespace iVectorOne.CSSuppliers.DerbySoft
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using iVectorOne.CSSuppliers.DerbySoft.Models;

    public class FeeCalculator
    {
        public decimal CalculateFees(List<Fee> fees, int totalPax, DateRange stayDateRange, List<decimal> amountsBeforeFees)
        {
            if (fees is null)
            {
                return 0;
            }

            return fees.Where(f => IsValidFee(f) && IsApplicableFee(f, stayDateRange))
            .Sum(fee => CalculateFee(fee, stayDateRange, totalPax, amountsBeforeFees));
        }

        private static bool IsValidFee(Fee fee) =>
            fee.FeeDetails is object
            && fee.FeeDetails.Type.HasValue
            && fee.FeeDetails.ChargeType.HasValue
            && fee.FeeDetails.AmountType.HasValue
            && fee.DateRange is object
            && fee.DateRange.StartDate.HasValue
            && fee.DateRange.EndDate.HasValue
            && fee.DateRange.StartDate != DateTime.MinValue
            && fee.DateRange.EndDate != DateTime.MinValue;

        private static bool IsApplicableFee(Fee fee, DateRange stayDateRange) =>
            (fee.FeeDetails != null && fee.FeeDetails.Type == FeeType.Exclusive)
            && (IsPerNight(fee) && ((fee.DateRange != null && fee.DateRange.StartDate <= stayDateRange.StartDate) || fee.DateRange.EndDate >= stayDateRange.EndDate))
            || (!IsPerNight(fee) && ((fee.DateRange != null && fee.DateRange.StartDate <= stayDateRange.StartDate) && fee.DateRange.EndDate >= stayDateRange.EndDate));

        private static decimal CalculateFee(Fee fee, DateRange stayDateRange, int totalPax, List<decimal> amountsBeforeFees)
        {
            var feeDateRange = new DateRange();

            if (fee.DateRange != null)
            {
                feeDateRange.StartDate = fee.DateRange.StartDate ?? new DateTime();
                feeDateRange.EndDate = fee.DateRange.EndDate ?? new DateTime();
            }

            bool isPerNight = IsPerNight(fee);
            int paxMultiplier = IsPerPerson(fee) ? totalPax : 1;

            decimal feeAmount = fee.FeeDetails?.Amount ?? 0m;
            var feeAmountType = fee.FeeDetails.AmountType.Value;

            if (isPerNight)
            {
                return CalculateNightlyFee(
                    stayDateRange,
                    amountsBeforeFees,
                    feeDateRange,
                    feeAmountType,
                    feeAmount,
                    paxMultiplier);
            }

            return CalculateFeeForAmountType(
                feeAmountType,
                feeAmount,
                amountsBeforeFees.Sum(),
                paxMultiplier);
        }

        private static decimal CalculateNightlyFee(
            DateRange stayDateRange,
            List<decimal> amountsBeforeFees,
            DateRange feeDateRange,
            FeeAmountType feeAmountType,
            decimal feeAmount,
            int paxMultiplier)
        {

            bool overlap = stayDateRange.StartDate <= feeDateRange.EndDate && feeDateRange.StartDate <= stayDateRange.EndDate;
            if (!overlap)
            {
                return 0;
            }

            var intersect = new DateRange
            (
                stayDateRange.StartDate > feeDateRange.StartDate ? stayDateRange.StartDate : feeDateRange.StartDate,
                stayDateRange.EndDate < feeDateRange.EndDate ? stayDateRange.EndDate : feeDateRange.EndDate
            );

            int startIndex = (intersect.StartDate - stayDateRange.StartDate).Days;
            int endIndex = (intersect.EndDate - stayDateRange.StartDate).Days;

            decimal feeTotal = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                feeTotal += CalculateFeeForAmountType(feeAmountType, feeAmount, amountsBeforeFees[i], paxMultiplier);
            }

            return feeTotal;
        }

        private static decimal CalculateFeeForAmountType(FeeAmountType amountType, decimal feeAmount, decimal applicableAmount, int paxMultiplier)
        {
            return amountType == FeeAmountType.Percent
                ? (feeAmount / 100m) * applicableAmount
                : feeAmount * paxMultiplier;
        }

        private static bool IsPerNight(Fee fee)
        {
            return (fee.FeeDetails != null && fee.FeeDetails.ChargeType == ChargeType.PerPersonPerNight)
                   || fee.FeeDetails.ChargeType == ChargeType.PerRoomPerNight;
        }

        private static bool IsPerPerson(Fee fee) =>
            (fee.FeeDetails != null && fee.FeeDetails.ChargeType == ChargeType.PerPersonPerNight)
            || fee.FeeDetails.ChargeType == ChargeType.PerPersonPerStay;

        public class DateRange
        {
            public DateTime StartDate;
            public DateTime EndDate;

            public DateRange() { }

            public DateRange(DateTime startDate, DateTime endDate)
            {
                StartDate = startDate;
                EndDate = endDate;
            }
        }
    }
}