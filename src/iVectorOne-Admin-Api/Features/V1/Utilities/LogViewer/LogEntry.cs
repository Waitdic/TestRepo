namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewer
{
    public record LogEntry
    {
        public string Environment { get; set; } = string.Empty;

        public string Timestamp { get; set; } = string.Empty;

        public string SupplierName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string ResponseTime { get; set; } = string.Empty;

        public string SupplierReference { get; set; } = string.Empty;

        public string LeadGuest { get; set; } = string.Empty;

    }
}
