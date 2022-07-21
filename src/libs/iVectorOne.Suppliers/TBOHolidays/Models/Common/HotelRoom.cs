namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    public class HotelRoom
    {
        public int RoomIndex { get; set; }

        public string RoomTypeName { get; set; } = string.Empty;

        public string RoomTypeCode { get; set; } = string.Empty;

        public string RatePlanCode { get; set; } = string.Empty;

        public RoomRate RoomRate { get; set; } = new();

        public Supplements Supplements { get; set; } = new();

        public decimal Discount { get; set; }
    }
}
