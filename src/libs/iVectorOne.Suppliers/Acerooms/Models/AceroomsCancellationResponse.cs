namespace ThirdParty.CSSuppliers.AceRooms.Models
{
    public class AceroomsCancellationResponse
    {
        public AuditDetails AuditData { get; set; } = new AuditDetails();

        public BookingDetails Booking { get; set; } = new BookingDetails();

        public class AuditDetails
        {
            public string CancelRef { get; set; } = string.Empty;
        }

        public class BookingDetails
        {
            public string Status { get; set; } = string.Empty;
            public decimal CancellationAmount { get; set; }
        }
    }
}
