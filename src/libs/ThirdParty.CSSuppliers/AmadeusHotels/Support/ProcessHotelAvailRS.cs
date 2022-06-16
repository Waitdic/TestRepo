namespace ThirdParty.CSSuppliers.AmadeusHotels.Support
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using ThirdParty.Models;
    using ThirdParty.Models.Property.Booking;
    using Models;
    using Models.Common;
    using Models.Soap;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;

    public class ProcessHotelAvailRS
    {
        public readonly EnvelopeResponse<OTAHotelAvailRS> HotelAvailResponse;
        private List<RoomStay> _roomStays = new();
        private RoomRate _roomRateNode;
        private RoomStay _roomStayNode;

        public ProcessHotelAvailRS(XmlDocument hotelAvailResponse, ISerializer serializer)
        {
            if (hotelAvailResponse == null)
            {
                throw new ArgumentNullException(nameof(hotelAvailResponse));
            }

            HotelAvailResponse = serializer.DeSerialize<EnvelopeResponse<OTAHotelAvailRS>>(hotelAvailResponse);
        }

        public bool FindLivePropertyNodes(
            string bookingCode,
            string ratePlanCode,
            string roomTypeCode,
            bool isRoomTypeConverted,
            string mealBasisCode,
            decimal expectedCost)
        {
            _roomStays = HotelAvailResponse.Body.Content.RoomStays.RoomStayList;
            RateNoteBuilder(x => true, r => r.BookingCode == bookingCode && r.RatePlanCode == ratePlanCode);

            if (_roomStayNode == null)
            {
                RateNoteBuilder(x => true, r => r.BookingCode == bookingCode);
            }

            if (_roomStayNode != null) return true;

            // Booking code can change for aggregators so just check to see if room with matching room type and rate plan and cost exists
            var mealBasisRestriction = mealBasisCode != "14"
                    ? x => x.RatePlans.Any(m => m.MealsIncluded.MealPlanCodes == mealBasisCode)
                    : new Func<RoomStay, bool>(x => true);

            if (isRoomTypeConverted)
            {
                RateNoteBuilder(x => x.RoomTypes.Any(r => r.RoomTypeAttribute == roomTypeCode) && mealBasisRestriction.Invoke(x),
                    p => p.RatePlanCode == ratePlanCode && p.Total.AmountAfterTax == expectedCost);

                if (_roomStayNode == null)
                {
                    RateNoteBuilder(x => x.RoomTypes.Any(r => r.RoomTypeAttribute == roomTypeCode) && mealBasisRestriction.Invoke(x),
                        p => p.RatePlanCode == ratePlanCode && p.Total.AmountBeforeTax == expectedCost);
                }
            }
            else
            {
                RateNoteBuilder(x => mealBasisRestriction.Invoke(x),
                    p => p.RatePlanCode == ratePlanCode
                         && p.RoomTypeCode == roomTypeCode
                         && p.Total.AmountAfterTax == expectedCost);

                if (_roomStayNode == null)
                {
                    RateNoteBuilder(x => mealBasisRestriction.Invoke(x),
                        p => p.RatePlanCode == ratePlanCode
                             && p.RoomTypeCode == roomTypeCode
                             && p.Total.AmountBeforeTax == expectedCost);
                }
            }

            return _roomStayNode != null;
        }

        public bool FindPricePropertyNodes(string roomTypeCode, string ratePlanCode, bool isRoomTypeConverted)
        {
            if (isRoomTypeConverted)
            {
                RateNoteBuilder(x => x.RoomTypes.Any(r => r.RoomTypeAttribute == roomTypeCode),
                   p => p.RatePlanCode == ratePlanCode);
            }
            else
            {
                RateNoteBuilder(x => true,
                    p => p.RoomTypeCode == roomTypeCode && p.RatePlanCode == ratePlanCode);
            }

            return _roomStays != null;
        }

        public bool IsRoomTypeConverted()
        {
            return _roomStayNode != null && _roomStayNode.RoomTypes.Select(x => x.IsConverted).FirstOrDefault();
        }

        public string AvailabilityStatus()
        {
            return _roomStayNode?.RoomRates.Select(x => x.AvailabilityStatus).FirstOrDefault() ?? string.Empty;
        }

        public string GetNewBookingCode()
        {
            return _roomStayNode?.RoomRates.Select(x => x.BookingCode).FirstOrDefault() ?? string.Empty;
        }

        public string SetPaymentCode()
        {
            return _roomRateNode?.Rates.Select(x => x.PaymentPolicies.GuaranteePayment.PaymentCode).FirstOrDefault() ?? string.Empty;
        }

        public Cancellations GetCancellations(PropertyDetails propertyDetails, List<string> errata)
        {
            var cancellations = new Cancellations();
            var cancelPenaltiesNode = _roomStayNode.RatePlans.Select(x => x.CancelPenalties).First();

            if (!cancelPenaltiesNode.Any()) return cancellations;

            foreach (var cancelPenalty in cancelPenaltiesNode)
            {
                decimal percentage = cancelPenalty.AmountPercent?.Percent ?? 100;
                var endDate = cancelPenalty.Deadline.AbsoluteDeadline == default
                    ? new DateTime(2099, 1, 1)
                    : cancelPenalty.Deadline.AbsoluteDeadline;
                decimal? amount = cancelPenalty.AmountPercent?.Amount;
                int? nights = cancelPenalty.AmountPercent?.NmbrOfNights;
                decimal feeAmount = 0;

                if (nights > 0)
                {
                    feeAmount = propertyDetails.Rooms[0].LocalCost / (decimal)nights;
                }
                else if (percentage > 0)
                {
                    feeAmount = propertyDetails.Rooms[0].LocalCost / (percentage / 100);
                }
                else if (amount > 0)
                {
                    feeAmount = amount.Value;
                }

                cancellations.AddNew(DateTime.Now, endDate, feeAmount);
                string penaltyDescription = cancelPenalty.PenaltyDescription.Text;

                if (!string.IsNullOrEmpty(penaltyDescription) && !errata.Contains(penaltyDescription))
                {
                    errata.Add(penaltyDescription);
                    propertyDetails.Errata.AddNew("Cancel Penalty Description", penaltyDescription);
                }
            }

            cancellations = Cancellations.MergeMultipleCancellationPolicies(cancellations);
            propertyDetails.Cancellations.Solidify(SolidifyType.Sum);

            return cancellations;
        }

        /// <summary>
        /// Gets the total cost.
        /// If any changes are made here please check AmadeusHotelsSearchXSL.xsl
        /// </summary>
        public decimal GetTotalCost(out bool errataRequired)
        {
            decimal totalAmountAfterTax = _roomRateNode.Total.AmountAfterTax;
            decimal totalAmountBeforeTax = _roomRateNode.Total.AmountBeforeTax;
            decimal baseAmountAfterTax = _roomRateNode.Rates.Select(x => x.Base.AmountAfterTax).First();
            decimal baseAmountBeforeTax = _roomRateNode.Rates.Select(x => x.Base.AmountBeforeTax).First();

            // 2019/08/29 Amadeus send the rates in the wrong order to the cost so the total is wrong.
            // It is correct if only one rate is returned 
            int baseRateCount = _roomRateNode.Rates.Length;

            // if Tax is not included add Errata
            errataRequired = true;

            if (totalAmountAfterTax != 0)
            {
                errataRequired = false;
                return totalAmountAfterTax;
            }

            if (totalAmountBeforeTax != 0)
            {
                return totalAmountBeforeTax;
            }

            if (baseRateCount != 1) return 0;

            if (baseAmountAfterTax != 0)
            {
                errataRequired = false;
                return CalculateTotalFromDailyRate(baseAmountAfterTax);
            }

            if (baseAmountBeforeTax != 0)
            {
                return CalculateTotalFromDailyRate(baseAmountBeforeTax);
            }

            return 0;
        }

        private decimal CalculateTotalFromDailyRate(decimal baseAmount)
        {
            string rateTimeUnit = _roomRateNode.Rates.Select(x => x.RateTimeUnit).First();
            if (rateTimeUnit == "Day" || string.IsNullOrEmpty(rateTimeUnit))
            {
                var effectiveDate = _roomRateNode.Rates.Select(x => x.EffectiveDate).ToSafeDate();
                var expireDate = _roomRateNode.Rates.Select(x => x.ExpireDate).ToSafeDate();

                long days = (effectiveDate.Date - expireDate.Date).Days;

                baseAmount *= days;
            }

            return baseAmount;
        }

        private void RateNoteBuilder(
            Func<RoomStay, bool> stayExp,
            Func<RoomRate, bool> rateExp)
        {
            _roomStayNode = _roomStays
                .Where(stayExp)
                .First(r => r.RoomRates.Any(rateExp)) ?? null!;

            _roomRateNode = _roomStayNode?.RoomRates.First()!;
        }
    }
}
