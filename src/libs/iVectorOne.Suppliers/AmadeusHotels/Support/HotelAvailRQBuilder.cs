namespace iVectorOne.Suppliers.AmadeusHotels.Support
{
    using System.Collections.Generic;
    using System.Linq;
    using Models.Common;
    using iVectorOne.Models.Property.Booking;

    public class HotelAvailRQBuilder
    {
        public static RatePlanCandidates BuildRatePlanCandidate(IEnumerable<RoomDetails> roomDetails)
        {
            // AmadeusHotels Distribution(GDS) only allows search for multiple identical occupancy rooms
            return new RatePlanCandidates
            {
                RatePlanCandidate = roomDetails
                    .Select(x => x.ThirdPartyReference.Split('|')[1])
                    .Distinct()
                    .Select(x => new RatePlanCandidate { RatePlanCode = x })
                    .ToArray()
            };
        }

        public static RoomStayCandidates BuildRoomStayCandidate(IEnumerable<RoomDetails> roomDetails, bool includeRoomTypeCode)
        {
            // AmadeusHotels Distribution(GDS) only allows search for multiple identical occupancy rooms
            return new RoomStayCandidates
            {
                RoomStayCandidate = roomDetails.Select((room, count) => new RoomStayCandidate
                {
                    RoomTypeCode = includeRoomTypeCode ? room.RoomTypeCode : string.Empty,
                    RoomID = count + 1,
                    Quantity = roomDetails.Count(),
                    GuestCounts =
                    {
                        IsPerRoom = "true",
                        GuestCount = { AgeQualifyingCode = "10", Count = room.Adults + room.Children }
                    }
                }).ToArray()
            };
        }

        public static RoomStayCandidates BuildSingleRoomStayCandidate(RoomDetails roomDetails)
        {
            // AmadeusHotels Distribution(GDS) only allows search for multiple identical occupancy rooms
            return new RoomStayCandidates
            {
                RoomStayCandidate = new[]
                {
                   new RoomStayCandidate
                   {
                       RoomTypeCode = roomDetails.RoomTypeCode,
                       Quantity = 1,
                       BookingCode = roomDetails.ThirdPartyReference.Split('|')[0],
                       GuestCounts =
                       {
                           IsPerRoom = "true",
                           GuestCount =
                           {
                               AgeQualifyingCode = "10",
                               Count = roomDetails.Adults + roomDetails.Children
                           }
                       }
                   }
               }
            };
        }
    }
}