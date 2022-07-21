namespace iVectorOne.CSSuppliers.Models.Yalago
{
#pragma warning disable CS8618

    public class YalagoLocalCharge
    {
        public LocalChargeAmount Amount { get; set; }
    }
    public class LocalChargeAmount
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}