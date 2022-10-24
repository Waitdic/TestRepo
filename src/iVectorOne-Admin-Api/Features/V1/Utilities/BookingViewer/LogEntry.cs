namespace iVectorOne_Admin_Api.Features.V1.Utilities.BookingViewer
{
    public record LogEntry
    {
        public string Environment { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int ResponseTime { get; set; }

        public string SupplierReference { get; set; } = string.Empty;

        public string LeadGuest { get; set; } = string.Empty;

    }
}
