﻿namespace iVectorOne_Admin_Api.Features.V1.Utilities.BookingViewer
{
    public record LogEntry
    {
        public string Environment { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int ResponseTime { get; set; }

        public string SupplierBookingReference { get; set; } = string.Empty;

        public string LeadGuestName { get; set; } = string.Empty;

        public int APILogId { get; set; }

        public bool Succesful { get; set; }
    }
}
