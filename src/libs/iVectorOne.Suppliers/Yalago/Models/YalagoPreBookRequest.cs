namespace ThirdParty.CSSuppliers.Models.Yalago
{
#pragma warning disable CS8618 

    class YalagoPreBookRequest
    {
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public int LocationId { get; set; }
        public int EstablishmentId { get; set; }
        public Room[] Rooms { get; set; }
        public string SourceMarket { get; set; }
        public string Culture { get; set; }
        public bool GetPackagePrice { get; set; }
        public bool GetBoardBoardBasis { get; set; }
        public bool GetTaxBreakDown { get; set; }
        public bool GetLocalCharges { get; set; }

        public class Room
        {
            public int Adults { get; set; }
            public int[] ChildAges { get; set; }
            public string RoomCode { get; set; }
            public string BoardCode { get; set; }
        }



    }
}
