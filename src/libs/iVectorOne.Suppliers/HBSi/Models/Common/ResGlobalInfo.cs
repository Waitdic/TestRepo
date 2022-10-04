namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    public class ResGlobalInfo
    {
        [XmlArray("HotelReservationIDs")]
        [XmlArrayItem("HotelReservationID")]
        public List<HotelReservationId> HotelReservationIds { get; set; } = new();
        public bool ShouldSerializeHotelReservationIds() => HotelReservationIds.Any();

        [XmlArray("Comments")]
        [XmlArrayItem("Comment")]
        public List<Comment> Comments { get; set; } = new();

        [XmlElement("DepositPayments")]
        public DepositPayments DepositPayments { get; set; } = new();
        public bool ShouldSerializeDepositPayments() => DepositPayments.RequiredPayment.AcceptedPayments.Any();
    }
}
