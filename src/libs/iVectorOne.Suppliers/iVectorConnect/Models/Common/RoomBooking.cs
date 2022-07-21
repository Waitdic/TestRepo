namespace ThirdParty.CSSuppliers.iVectorConnect.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomBooking
    {
        public string RoomBookingToken { get; set; } = string.Empty;

        public GuestConfiguration GuestConfiguration { get; set; } = new();

        [XmlArray("GuestIDs")]
        [XmlArrayItem("GuestID")]
        public int[] GuestIDs { get; set; } = Array.Empty<int>();
    }
}
