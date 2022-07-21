namespace iVectorOne.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4
{
    using System.Collections.Generic;
    using System.Linq;
    using iVectorOne.CSSuppliers.DerbySoft.DerbySoftBookingUsbV4.Models;

    public class HotelValidator
    {
        public static List<string> HotelsWithCompleteRoomSelection(List<string> hotelList, int roomCount, 
            List<System.Tuple<int, DerbySoftBookingUsbV4AvailabilityResponse>> responses)
        {
            var validHotels = new List<string>();
            validHotels.AddRange(hotelList);

            for (var i = 1; i <= roomCount; i++)
            {
                validHotels.RemoveAll(h => !responses.Any(r => (r.Item1 == i) && r.Item2.HotelId == h));
            }

            return validHotels;
        }
    }
}
