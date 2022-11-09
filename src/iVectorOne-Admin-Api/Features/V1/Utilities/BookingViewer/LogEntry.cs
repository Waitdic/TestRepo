namespace iVectorOne_Admin_Api.Features.V1.Utilities.BookingViewer
{
    public record LogEntry
    {
        public string Environment { get; set; } = string.Empty;

        public string Timestamp { get; set; } = string.Empty;

        public string SupplierName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string ResponseTime { get; set; } = string.Empty;

        public string SupplierBookingReference { get; set; } = string.Empty;

        public string LeadGuestName { get; set; } = string.Empty;

        public int APILogId { get; set; }

        public int SupplierApiLogId { get; set; }

        public bool Succesful { get; set; }
    }
}
