namespace iVectorOne.Models.Logging
{
    using System;

    public class APILog
    {
        public int APILogID { get; set; }
        public LogType Type { get; set; }
        public DateTime Time { get; set; }
        public string RequestLog { get; set; } = string.Empty;
        public string ResponseLog { get; set; } = string.Empty;
        public int AccountID { get; set; }
        public bool Success { get; set; }
        public int BookingID { get; set; }
    }
}