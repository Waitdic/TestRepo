namespace iVectorOne.CSSuppliers.FastPayHotels.Models
{
    using System.Collections.Generic;

    public class FastPayHotelsPrebookResponse
    {
        public string messageID { get; set; } = string.Empty;
        public Result result { get; set; } = new Result();

        public class Result
        {
            public bool success { get; set; }
            public string message { get; set; } = string.Empty;
            public List<string> reservationTokens { get; set; } = new List<string>();
        }
     }
}