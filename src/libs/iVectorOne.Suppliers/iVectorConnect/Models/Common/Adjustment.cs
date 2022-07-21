namespace iVectorOne.Suppliers.iVectorConnect.Models.Common
{
    public class Adjustment
    {
        public int AdjustmentID { get; set; }
        public string AdjustmentName { get; set; } = string.Empty;
        public string AdjustmentType { get; set; } = string.Empty;
        public bool PayLocal { get; set; }
        public bool Optional { get; set; }
        public decimal Total { get; set; }
        public bool NonMarginable { get; set; }
        public bool Commissionable { get; set; }
    }
}