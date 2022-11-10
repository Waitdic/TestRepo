namespace iVectorOne.Models.Logging
{
    using System;

    public class SupplierAPILog
    {
        public int SupplierAPILogID { get; set; }
        public int AccountID { get; set; }
        public int SupplierID { get; set; }
        public LogType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime RequestDateTime { get; set; }
        public int ResponseTime { get; set; }
        public bool Successful { get; set; }
        public string RequestLog { get; set; } = string.Empty;
        public string ResponseLog { get; set; } = string.Empty;
        public int BookingID { get; set; }
    }
}