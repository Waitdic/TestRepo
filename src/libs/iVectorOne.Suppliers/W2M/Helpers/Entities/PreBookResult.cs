using System.Collections.Generic;

namespace iVectorOne.Suppliers.Models.W2M
{
#pragma warning disable CS8618

    public class PreBookResult
    {
        public bool Success { get; set; }
        public Dictionary<string, string> Warnings { get; set; } = new Dictionary<string, string>();
        public List<PreBookCancellation> Cancellations { get; set; } = new List<PreBookCancellation>();
        public List<PreBookErrata> Errata { get; set; } = new List<PreBookErrata>();
        public List<string> BookingCodes { get; set; } = new List<string>();
        public List<BookLog> Logs { get; set; } = new List<BookLog>();
    }
}
