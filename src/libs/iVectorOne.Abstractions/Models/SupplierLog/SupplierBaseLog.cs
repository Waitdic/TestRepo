using Intuitive.Helpers.Net;

namespace iVectorOne.Models.SupplierLog
{
    public class SupplierBaseLog
    {
        public string SupplierName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public Request Request { get; set; } = null!;
    }
}
