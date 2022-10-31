using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace iVectorOne_Admin_Api.Data.Models
{
    public class BookingLog
    {
        public int BookingId { get; set; }

        public DateTime BookingDateTime { get; set; }

        public string SupplierName { get; set; }    = string.Empty;

        public string Type { get; set; } = string.Empty;

        public bool Success { get; set; }

        public int ResponseTime { get; set; }

        public string SupplierBookingReference { get; set; } = string.Empty;

        public string LeadGuestName { get; set; } = string.Empty;

    }
}
