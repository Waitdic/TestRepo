namespace iVectorOne.Suppliers.OceanBeds.Models.Common
{
    public class Booking
    {
        public string CheckInDate { get; set; } = string.Empty;
        public string CheckOutDate { get; set;} = string.Empty;
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }
        public string RoomId { get; set; } = string.Empty;
        public string NetPrice { get; set; } = string.Empty;
        public string SpecialRequest { get; set; } = string.Empty;
        public string BookingType { get; set; } = string.Empty;
    }
}
