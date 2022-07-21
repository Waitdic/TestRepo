namespace iVectorOne.CSSuppliers.RMI.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class BookDetails
    {
        public string ArrivalDate { get; set; } = string.Empty;

        public int Duration { get; set; }

        public LeadGuest LeadGuest { get; set; }

        public string TradeReference { get; set; } = string.Empty;

        [XmlArray("RoomBookings")]
        [XmlArrayItem("RoomBooking")]
        public List<RoomBooking> RoomBookings { get; set; } = new();


    }
}
