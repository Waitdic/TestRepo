namespace ThirdParty.CSSuppliers.Models.Yalago
{
#pragma warning disable CS8618

    class YalagoCreateBookingResponse
    {
        public string BookingRef { get; set; }
        public int Status { get; set; }
        public Establishment establishment { get; set; }
        public Room[] Rooms { get; set; }
        public InfoItem[] InfoItems { get; set; }
        public string ErrorMessage { get; set; }
        public string[] Error { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public string AffiliateRef { get; set; }
        public Booking[] Bookings { get; set; }

    }
    public class Establishment
    {
        public int EstablishmentId { get; set; }
        public string Name { get; set; }
        public string EstablishmentInfo { get; set; }
    }
    public class Room
    {
        public int BookRoomId { get; set; }
        public string AffiliateRoomRef { get; set; }
        public string Description { get; set; }
        public string Board { get; set; }
        public string RoomCode { get; set; }
        public string BoardCode { get; set; }
        public string ProviderRef { get; set; }
        public string ProviderName { get; set; }
        public Guest[] Guests { get; set; }
        public Cost grossCost { get; set; }
        public Cost netCost { get; set; }
        public string SpecialRequests { get; set; }
        public bool NonRefundable { get; set; }
        public Extra[] Extras { get; set; }
        public bool IsBindingPrice { get; set; }
        public YalagoLocalCharge[] LocalCharges { get; set; }

    }
    public class InfoItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class Booking
    {
        public int BookingIndex { get; set; }
        public string BookingRef { get; set; }
        public int Status { get; set; }
        public Establishment establishment { get; set; }
        public Room[] Rooms { get; set; }
        public InfoItem[] InfoItems { get; set; }
        public string ErrorMessage { get; set; }
        public string[] Error { get; set; }
    }

    public class Guest
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class Cost
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public class Extra
    {
        public string Title { get; set; }
        public string Desription { get; set; }
    }
}