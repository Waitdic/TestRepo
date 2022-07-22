namespace iVectorOne.CSSuppliers.DerbySoft
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.CSSuppliers.DerbySoft.Models;
    using iVectorOne.Models.Property.Booking;

    public class CancellationCalculator
    {
        public Cancellations GetCancellations(CancelPolicy? cancelPolicy, List<decimal> dailyRates, DateTime arrivalDate)
        {
            var cancellations = new Cancellations();

            if(cancelPolicy != null)
            {
                cancellations = SortCancellations(cancelPolicy.CancelPenalties.Select(c => CalculateCancellation(c, dailyRates, arrivalDate)).ToList());
            }

            return cancellations;
        }

        public Cancellation CalculateCancellation(CancelPenalty cancelPenalty, List<decimal> dailyRates, DateTime arrivalDate)
        {
            if (!cancelPenalty.NoShow)
            {
                if (cancelPenalty.CancelDeadline.OffsetTimeDropType == OffsetTimeDropType.BeforeArrival)
                {
                    //default to arrivaldate as this is "BeforeArrival"
                    var startDate = arrivalDate;

                    //apply offset
                    if (cancelPenalty.CancelDeadline.OffsetTimeValue > 0)
                    {
                        switch (cancelPenalty.CancelDeadline.OffsetTimeUnit)
                        {
                            case OffsetTimeUnit.D:
                                startDate = arrivalDate.AddDays(-cancelPenalty.CancelDeadline.OffsetTimeValue);
                                break;
                            case OffsetTimeUnit.H:
                                startDate = arrivalDate.AddHours(-cancelPenalty.CancelDeadline.OffsetTimeValue);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    //any date time is booking date or use
                    //local deadline time for cancellation
                    if (!string.IsNullOrEmpty(cancelPenalty.CancelDeadline.DeadlineTime))
                    {
                        if (cancelPenalty.CancelDeadline.DeadlineTime.ToLower() == "any date time")
                        {
                            startDate = DateTime.Today;
                        }
                        else if (cancelPenalty.CancelDeadline.DeadlineTime.Contains("AM"))
                        {
                            int hour = new string(cancelPenalty.CancelDeadline.DeadlineTime.TakeWhile(char.IsDigit).ToArray()).ToSafeInt();
                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, hour, 0, 0);
                        }
                        else if (cancelPenalty.CancelDeadline.DeadlineTime.Contains("PM"))
                        {
                            int hour = new string(cancelPenalty.CancelDeadline.DeadlineTime.TakeWhile(char.IsDigit).ToArray()).ToSafeInt();
                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 12 + hour, 0, 0);
                        }
                    }

                    var cancellation = new Cancellation
                    {
                        StartDate = startDate,
                        EndDate = arrivalDate.AddDays(1),
                        Amount = CalculatePenaltyCharge(cancelPenalty.PenaltyCharge, dailyRates)
                    };
                    return cancellation;

                }

                //it should always be BeforeArrival
                return null;
            }

            return new Cancellation
            {
                //6am morning after checkin
                StartDate = arrivalDate.AddDays(1).AddHours(6),
                EndDate = new DateTime(2099, 1, 1),
                Amount = CalculatePenaltyCharge(cancelPenalty.PenaltyCharge, dailyRates)
            };
        }

        private static decimal CalculatePenaltyCharge(PenaltyCharge penaltyCharge, List<decimal> dailyRates)
        {
            decimal totalCost = dailyRates.Sum();

            if (penaltyCharge.ChargeBase == CancelChargeBase.FullStay)
            {
                return totalCost;
            }

            if (penaltyCharge.ChargeBase == CancelChargeBase.NightBase)
            {
                int numberOfNights = penaltyCharge.Nights ?? dailyRates.Count;
                return dailyRates.Take(numberOfNights).Sum();
            }

            if (penaltyCharge.ChargeBase == CancelChargeBase.Amount)
            {
                return (decimal)penaltyCharge.Amount;
            }

            if (penaltyCharge.Percent.HasValue)
            {
                return Math.Round((decimal)(totalCost * (penaltyCharge.Percent / 100)), 2);
            }

            //amount shouldn't be null here but if so use total cost
            return penaltyCharge.Amount ?? totalCost;
        }

        // Sums the cancellation policies
        // Uses date times instead of dates
        public Cancellations SolidifyCancellations(List<Cancellation> cancellations)
        {
            var finalCancellations = new Cancellations();

            if (cancellations.Count == 0)
            {
                return finalCancellations;
            }

            var startDates = new List<DateTime>();

            foreach (var cancellation in cancellations)
            {
                if (!startDates.Contains(cancellation.StartDate)) startDates.Add(cancellation.StartDate);
            }

            startDates.Sort();

            var maxEndDate = cancellations.Max(c => c.EndDate);

            decimal previousAmount = 0M;

            foreach (var startDate in startDates)
            {
                //get the policies active on this date
                var rulesInEffect = cancellations.Where(c => c.StartDate <= startDate && c.EndDate >= startDate).ToList();

                var newRule = new Cancellation {StartDate = startDate, Amount = rulesInEffect.Sum(r => r.Amount)};
                
                //if amount has changed add new policy
                if (newRule.Amount != previousAmount)
                {
                    finalCancellations.Add(newRule);
                }

                previousAmount = newRule.Amount;
            }

            //set the end dates
            if (finalCancellations.Count > 1)
            {
                for (var i = 0; i < finalCancellations.Count - 1; i++)
                {
                    finalCancellations[i].EndDate = finalCancellations[i + 1].StartDate.AddMinutes(-1);
                }
            }

            if (finalCancellations.Count > 0) finalCancellations.Last().EndDate = maxEndDate;

            return finalCancellations;
        }

        public Cancellations SortCancellations(List<Cancellation> cancellations)
        {
            var orderedCancellations = new Cancellations();

            orderedCancellations.AddRange(cancellations.OrderBy(c => c.StartDate).ToList());
                
            for (int iCanx = 0; iCanx < orderedCancellations.Count; iCanx++)
            {
                var cancellation = orderedCancellations[iCanx];
                if (iCanx < orderedCancellations.Count - 1)
                {
                    orderedCancellations[iCanx].EndDate = orderedCancellations[iCanx + 1].StartDate.AddMinutes(-1);
                }
            }

            return orderedCancellations;
        }
    }
}