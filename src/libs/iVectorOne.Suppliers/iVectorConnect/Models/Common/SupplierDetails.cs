namespace iVectorOne.Suppliers.iVectorConnect.Models.Common
{
    public class SupplierDetails
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string Source { get; set; }
        public int PropertyID { get; set; }
        public int CurrencyID { get; set; }
        public decimal Cost { get; set; }
        public decimal GrossCost { get; set; }
        public decimal BuyingCommission { get; set; }
        public string RateCode { get; set; }
        public string RateDescription { get; set; }
    }
}