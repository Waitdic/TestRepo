namespace iVectorOne.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Intuitive;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Models;

    /// <summary>
    /// A list of cancellation
    /// </summary>
    /// <seealso cref="List{Cancellation}" />
    public class Cancellations : List<Cancellation>
    {
        /// <summary>
        /// Solidifies the specified o policy.
        /// </summary>
        /// <param name="policy">The o policy.</param>
        /// <param name="solidifyType">Type of the e solidify.</param>
        /// <param name="solidifyTo">The d solidify to.</param>
        /// <param name="finalCost">The n final cost.</param>
        /// <returns>A list of cancellations</returns>
        public Cancellations Solidify(Cancellations policy, SolidifyType solidifyType, DateTime solidifyTo = new DateTime(), decimal finalCost = 0m)
        {
            if (solidifyTo == DateTime.Parse("1990-01-01"))
            {
                solidifyTo = DateTimeExtensions.EmptyDate;
            }

            // Create a new empty policy to put our rules in
            var finalPolicy = new Cancellations();

            // First of all, we need to get all the start dates in order
            var startDates = new List<DateTime>();
            foreach (Cancellation cancellation in policy)
            {
                if (cancellation != null && !startDates.Contains(cancellation.StartDate))
                {
                    startDates.Add(cancellation.StartDate);
                }
            }

            startDates.Sort();

            // Loop through the dates
            decimal lastAmount = -1m;
            foreach (DateTime startDate in startDates)
            {
                // get all the rules that pass over this date
                var lambdaCompareTo = startDate; // this is fine in fact as we're only doing a comparision, but I put this here to stop it from complaining
                List<Cancellation> rulesInEffect = policy.FindAll(oRule => oRule.StartDate <= lambdaCompareTo && oRule.EndDate >= lambdaCompareTo);

                // all we need now is to choose the correct amount to apply at this date band point
                var ruleToApply = new Cancellation(startDate, DateTimeExtensions.EmptyDate, 0);
                switch (solidifyType)
                {
                    case SolidifyType.Min:
                        {
                            if (rulesInEffect.Count != 0)
                            {
                                ruleToApply.Amount = rulesInEffect[0].Amount;
                            }

                            foreach (Cancellation rule in rulesInEffect)
                            {
                                if (rule.Amount < ruleToApply.Amount)
                                {
                                    ruleToApply.Amount = rule.Amount;
                                }
                            }

                            break;
                        }

                    case SolidifyType.Max:
                        {
                            if (rulesInEffect.Count != 0)
                            {
                                ruleToApply.Amount = rulesInEffect[0].Amount;
                            }

                            foreach (Cancellation rule in rulesInEffect)
                            {
                                if (rule.Amount > ruleToApply.Amount)
                                {
                                    ruleToApply.Amount = rule.Amount;
                                }
                            }

                            break;
                        }

                    case SolidifyType.Sum:
                        {
                            foreach (Cancellation rule in rulesInEffect)
                            {
                                ruleToApply.Amount += rule.Amount;
                            }

                            break;
                        }

                    case SolidifyType.LatestStartDate:
                        {
                            if (rulesInEffect.Count != 0)
                            {
                                ruleToApply.Amount = rulesInEffect[0].Amount;
                            }

                            DateTime latestStartDateSoFar = DateTimeExtensions.EmptyDate;
                            foreach (Cancellation rule in rulesInEffect)
                            {
                                if (rule.StartDate > latestStartDateSoFar || (rule.StartDate == latestStartDateSoFar && rule.Amount > ruleToApply.Amount))
                                {
                                    latestStartDateSoFar = rule.StartDate;
                                    ruleToApply.Amount = rule.Amount;
                                }
                            }

                            break;
                        }
                }

                // if the amount has actually changed, then add it in to the final policy
                if (ruleToApply.Amount != lastAmount)
                {
                    finalPolicy.Add(ruleToApply);
                }

                // store this amount to compare with next time round
                lastAmount = ruleToApply.Amount;
            }

            // Work out all the end dates except the last one
            DateTime lastEndDateFromOriginalPolicy = DateTimeExtensions.EmptyDate;
            DateTime firstStartDateFromOriginalPolicy = DateTimeExtensions.EmptyDate;
            foreach (Cancellation rule in policy)
            {
                if (rule.EndDate > lastEndDateFromOriginalPolicy)
                {
                    lastEndDateFromOriginalPolicy = rule.EndDate;
                }

                if (firstStartDateFromOriginalPolicy == DateTimeExtensions.EmptyDate)
                {
                    firstStartDateFromOriginalPolicy = rule.StartDate;
                } 

                if (rule.StartDate < firstStartDateFromOriginalPolicy)
                {
                    firstStartDateFromOriginalPolicy = rule.StartDate;
                }
            }

            if (finalPolicy.Count > 1)
            {
                for (int i = 0, loopTo = finalPolicy.Count - 2; i <= loopTo; i++)
                {
                    finalPolicy[i].EndDate = finalPolicy[i + 1].StartDate.AddDays(-1);
                }
            }

            // Do the last end date, and add a final date band if necessary
            if (finalPolicy.Count != 0)
            {
                if (solidifyTo != DateTimeExtensions.EmptyDate && lastEndDateFromOriginalPolicy < solidifyTo && (finalCost == 0m || finalCost == lastAmount))
                {
                    finalPolicy[finalPolicy.Count - 1].EndDate = solidifyTo;
                }
                else
                {
                    finalPolicy[finalPolicy.Count - 1].EndDate = lastEndDateFromOriginalPolicy;
                    if (solidifyTo != DateTimeExtensions.EmptyDate && lastEndDateFromOriginalPolicy < solidifyTo)
                    {
                        finalPolicy.AddNew(lastEndDateFromOriginalPolicy.AddDays(1d), solidifyTo, finalCost);
                    }
                }

                if (firstStartDateFromOriginalPolicy.Date > DateTime.Now.Date)
                {
                    finalPolicy.Insert(0, new Cancellation(DateTime.Now.Date, firstStartDateFromOriginalPolicy, 0));
                }
            }

            // Return the final policy
            return finalPolicy;
        }

        /// <summary>
        /// Merges the multiple cancellation policies.
        /// </summary>
        /// <param name="cancellationPolicies">a cancellation policies.</param>
        /// <returns>A list of cancellations</returns>
        public static Cancellations MergeMultipleCancellationPolicies(params Cancellations[] cancellationPolicies)
        {
            Cancellations mergedCancellations = new Cancellations();

            // Find the earliest end date of all the policies, and add all the rules into our final merged one
            DateTime earliestEndDate = DateTimeExtensions.EmptyDate;

            foreach (Cancellations policy in cancellationPolicies)
            {
                // find end date
                DateTime endDateForThisPolicy = DateTimeExtensions.EmptyDate;

                foreach (Cancellation rule in policy)
                {
                    if (rule.EndDate > endDateForThisPolicy)
                    {
                        endDateForThisPolicy = rule.EndDate;
                    }
                }

                if (earliestEndDate == DateTimeExtensions.EmptyDate || endDateForThisPolicy < earliestEndDate)
                {
                    earliestEndDate = endDateForThisPolicy;
                }

                // add the rules
                mergedCancellations.AddRange(policy);
            }

            // Get rid of any rules that start too late
            mergedCancellations.RemoveAll(oRule => oRule.StartDate > earliestEndDate);

            // Chop the end off of any rules that end too late
            foreach (Cancellation rule in mergedCancellations)
            {
                if (rule.EndDate > earliestEndDate)
                {
                    rule.EndDate = earliestEndDate;
                }
            }

            // Now solidify the rules (sum them all up)
            mergedCancellations.Solidify(SolidifyType.Sum);

            // Return our new merged policy
            return mergedCancellations;
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="amount">The amount.</param>
        public void AddNew(DateTime startDate, DateTime endDate, decimal amount)
        {
            this.Add(new Cancellation(startDate, endDate, amount));
        }

        /// <summary>
        /// Solidifies the specified e solidify type.
        /// </summary>
        /// <param name="solidifyType">Type of the e solidify.</param>
        /// <param name="solidifyTo">The d solidify to.</param>
        /// <param name="finalCost">The n final cost.</param>
        public void Solidify(SolidifyType solidifyType, DateTime solidifyTo = new DateTime(), decimal finalCost = 0m)
        {
            Cancellations finalPolicy = this.Solidify(this, solidifyType, solidifyTo, finalCost);
            this.Clear();
            this.AddRange(finalPolicy);
        }
    }
}