using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace iVectorOne_Admin_Api.Data.Models
{
    public class LogEntry
    {
        public int SupplierApiLogId { get; set; }

        public DateTime RequestDateTime { get; set; }

        public string SupplierName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public bool Successful { get; set; }

        public int ResponseTime { get; set; }

        public string? SupplierBookingReference { get; set; }

        public string? LeadGuestName { get; set; } 

    }
}
