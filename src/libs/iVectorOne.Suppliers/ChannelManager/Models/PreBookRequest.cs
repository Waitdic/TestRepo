namespace iVectorOne.CSSuppliers.ChannelManager.Models
{
    using System;
    using System.Collections.Generic;
    using iVectorOne.CSSuppliers.ChannelManager.Models.Common;

    public class PreBookRequest
    {
        public BookingLogin LoginDetails { get; set; } = new();

        public int BrandID { get; set; }
        public int PropertyReferenceID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public List<Room> Rooms { get; set; } = new();

        public class Room
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