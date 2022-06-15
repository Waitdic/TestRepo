namespace ThirdParty.CSSuppliers.iVectorChannelManager.Models
{
    using System;
    using System.Collections.Generic;

    public partial class PreBookRequest
    {

        public BookingLogin LoginDetails { get; set; }

        public int BrandID { get; set; }
        public int PropertyReferenceID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<Room> Rooms { get; set; }

        public partial class Room
        {
            public int Seq { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
            public string ChildAgeCSV { get; set; }
            public string RoomBookingToken { get; set; }
            public int PropertyRoomTypeID { get; set; }
            public int MealBasisID { get; set; }
            public int PropertyID { get; set; }
            public int BrandID { get; set; }
        }


    }
}