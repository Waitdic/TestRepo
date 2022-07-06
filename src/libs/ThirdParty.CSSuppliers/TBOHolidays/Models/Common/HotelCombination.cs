namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    using System;
    using System.Linq;

    public class HotelCombination
    {
        public string HotelCode { get; set; } = string.Empty;

        public string CombinationType { get; set; } = string.Empty;

        public Room[] Rooms { get; set; } = Array.Empty<Room>();
        public bool ShouldSerializeRooms() => Rooms.Any();
    }
}
