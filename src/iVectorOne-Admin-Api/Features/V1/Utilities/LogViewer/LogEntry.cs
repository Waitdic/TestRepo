namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewer
{
    public record LogEntry
    {
        public int SupplierApiLogId { get; set; }

        public string Environment { get; set; } = string.Empty;

        public string Timestamp { get; set; } = string.Empty;

        public string SupplierName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string ResponseTime { get; set; } = string.Empty;

        public string SupplierBookingReference { get; set; } = string.Empty;

        public string LeadGuestName { get; set; } = string.Empty;

        public bool Succesful { get; set; }

    }
}
