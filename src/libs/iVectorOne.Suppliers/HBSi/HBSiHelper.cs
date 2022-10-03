namespace iVectorOne.Suppliers.HBSi
{
    using Intuitive.Helpers.Extensions;
    using iVector.Search.Property;
    using iVectorOne.Models;
    using iVectorOne.Search.Models;
    using iVectorOne.Suppliers.HBSi.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class HBSiHelper
    {
        public static int GetDefaultAge(PassengerType passengerType, int Age = 0)
        {
            switch (passengerType)
            {
                case PassengerType.Child:
                    return 10;
                case PassengerType.Infant:
                    return 1;
                default:
                    return 30;
            }
        }

        public static string GetAgeQualifyingCode(PassengerType passengerType)
        {
            switch (passengerType)
            {
                case PassengerType.Child:
                    return "8";
                case PassengerType.Infant:
                    return "7";
                default:
                    return "10";
            }
        }

        public static List<GuestCount> GetDefaultAgesGuestCounts(RoomDetail oRoomDetail)
        {
            var guestCounts = new List<GuestCount>();
            if (oRoomDetail.Adults > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    Age = GetDefaultAge(PassengerType.Adult),
                    Count = oRoomDetail.Adults
                });
            }
            if (oRoomDetail.Children > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    Age = GetDefaultAge(PassengerType.Child),
                    Count = oRoomDetail.Children
                });
            }
            if (oRoomDetail.Infants > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    Age = GetDefaultAge(PassengerType.Infant),
                    Count = oRoomDetail.Infants
                });
            }
            return guestCounts;
        }

        public static List<GuestCount> GetQualifyingAgeGuestCounts(RoomDetail oRoomDetail)
        {
            var guestCounts = new List<GuestCount>();
            if (oRoomDetail.Adults > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    AgeQualifyingCode = GetAgeQualifyingCode(PassengerType.Adult),
                    Count = oRoomDetail.Adults
                });
            }
            if (oRoomDetail.Children > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    AgeQualifyingCode = GetAgeQualifyingCode(PassengerType.Child),
                    Count = oRoomDetail.Children
                });
            }
            if (oRoomDetail.Infants > 0)
            {
                guestCounts.Add(new GuestCount
                {
                    AgeQualifyingCode = GetAgeQualifyingCode(PassengerType.Infant),
                    Count = oRoomDetail.Infants
                });
            }
            return guestCounts;
        }

        public static string GuestCountedRoomIdenty(RoomDetail oRoomDetail)
        {
            return $"{oRoomDetail.Adults}:{oRoomDetail.Children}:{oRoomDetail.Infants}:{string.Join(",", oRoomDetail.ChildAges.OrderBy(age => age))}";
        }

        public static string GuestCountIdenty(GuestCount oGuestCount)
        {
            return "";
        }

        public static int GetRoomCapacity(string sMaxRoomOcupancy, string sHotelCode, string sRoomTypeCode)
        {
            // restrictions
            var guestCountLimits = sMaxRoomOcupancy.Split(',')
                .Where(x => x.Split('#').Count() == 3)
                .Select(sHotelRoomOccupancy =>
                {
                    string[] codes = sHotelRoomOccupancy.Split('#');
                    return new
                    {
                        HotelCode = codes[0],
                        RoomTypeCode = codes[1],
                        MaxOccupancy = codes[2].ToSafeInt()
                    };
                }).ToList();

            int maxOccupancy = guestCountLimits.FirstOrDefault(x => string.Equals(x.HotelCode, sHotelCode)
                                                                 && string.Equals(x.RoomTypeCode, sRoomTypeCode))?
                                               .MaxOccupancy ?? 0;
            return maxOccupancy;
        }
        public static List<GuestCount> GetGuestCounts(bool bUsePassangerAge, RoomDetail oRoomDetail)
        {
            return (bUsePassangerAge
                ? GetDefaultAgesGuestCounts(oRoomDetail)
                : GetQualifyingAgeGuestCounts(oRoomDetail)).ToList();
        }

        public static int GetGuestAgeCount(List<GuestCount> oGuestCounts, PassengerType ePassengerType, bool bUsePassangerAge)
        {
            return bUsePassangerAge
                ? oGuestCounts.FirstOrDefault(x => x.Age == GetDefaultAge(ePassengerType))?.Count ?? 0
                : oGuestCounts.FirstOrDefault(x => string.Equals(x.AgeQualifyingCode, GetAgeQualifyingCode(ePassengerType)))?.Count ?? 0;
        }

        public static string GetRoomIdentyCode(RoomStay oRoomStay, bool bUsePassangerAge)
        {
            string roomTypeCode = oRoomStay.RoomRates.First().RoomTypeCode;
            string ratePlanCode = oRoomStay.RoomRates.First().RatePlanCode;
            string currencyCode = oRoomStay.RoomRates.First().Rates.First().Total.CurrencyCode;
            string mealPlanCode = oRoomStay.RatePlans.First().MealsIncluded.MealPlanCodes;
            int iAdults = GetGuestAgeCount(oRoomStay.GuestCounts, PassengerType.Adult, bUsePassangerAge);
            int iChild = GetGuestAgeCount(oRoomStay.GuestCounts, PassengerType.Child, bUsePassangerAge);
            int iInfants = GetGuestAgeCount(oRoomStay.GuestCounts, PassengerType.Infant, bUsePassangerAge);

            return $"{roomTypeCode}|{ratePlanCode}|{mealPlanCode}|{iAdults}|{iChild}|{iInfants}|{currencyCode}";
        }

        public static List<RoomStayCandidate> GetRoomStayCandidates(SearchDetails oSearchDetails, IHBSiSettings oSettings, string source)
        {
            //'if we have multiple room details then only add the ones with distinct guest counts
            var roomStayCandidates = oSearchDetails.RoomDetails.GroupBy(GuestCountedRoomIdenty)
                .Select((grRoomDetails, iRoomIndex) =>
                {
                    var oRoomDetail = grRoomDetails.First();
                    var oRoomStayCandidate = new RoomStayCandidate
                    {
                        RoomTypeCode = "*",
                        RPH = iRoomIndex + 1,
                        RatePlanCandidateRPH = 1,
                        Quantity = grRoomDetails.Count(),
                        GuestCounts = GetGuestCounts(oSettings.UsePassengerAge(oSearchDetails, source), oRoomDetail)
                    };
                    return oRoomStayCandidate;
                }).ToList();
            return roomStayCandidates;
        }

        public static bool GuestSetEquals(List<GuestCount> set1, List<GuestCount> set2)
        {
            bool equals = set1.Select((g) => new { Code = $"{g.Age}:{g.AgeQualifyingCode}:{g.Count}", Set = 1 })
                .Concat(set2.Select((g) => new { Code = $"{g.Age}:{g.AgeQualifyingCode}:{g.Count}", Set = 2 }))
                .GroupBy(x => x.Code).Select(gr => new { Sum = gr.Sum(x => x.Set), Count = gr.Count() })
                .All(x => x.Sum == 3 && x.Count == 2);
            return equals;
        }

        public static Pos GetPos(IThirdPartyAttributeSearch TPAttributeSearch, IHBSiSettings settings, string source)
        {
            return new Pos
            {
                Source =
                    {
                        RequestorID =
                        {
                            TypeCode = "18",
                            ID = source
                        },
                        BookingChannel =
                        {
                            ChannelType = "2",
                            Primary = "true",
                            CompanyName = settings.PartnerName(TPAttributeSearch, source)
                        }
                    }
            };
        }

        public static DateTime Now()
        {
            var now = DateTime.Now;
#if DEBUG
            now = Constant.UnitTestDateTime;
#endif
            return now;
        }
    }
}
