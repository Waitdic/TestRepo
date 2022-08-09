namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System;

    public class CancelPeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal FeePercent { get; set; }
    }
}
