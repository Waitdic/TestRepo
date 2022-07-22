using System.Collections.Generic;

namespace iVectorOne.CSSuppliers.Models.W2M
{
#pragma warning disable CS8618

    public class BookResult
    {
        public bool Success { get; set; }
        public List<string> References { get; set; } = new List<string>();
        public Dictionary<string, string> Warnings { get; set; } = new Dictionary<string, string>();
        public List<BookLog> Logs { get; set; } = new List<BookLog>();
    }
}
