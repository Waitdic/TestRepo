namespace iVectorOne.Models.Transfer
{
    using System;

    public class TransferBookingDetails
    {
        public int TransferBookingID { get; set; }
        public string Source { get; set; } = string.Empty;
        public int SupplierID { get; set; }
        public DateTime DepartureDate { get; set; }
        public int ISOCurrencyID { get; set; }
    }
}