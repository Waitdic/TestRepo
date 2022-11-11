namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class HotelReservationId
    {
        [XmlAttribute("ResID_Type")]
        public string ResIdType { get; set; } = string.Empty;

        [XmlAttribute("ResID_Value")]
        public string ResIdValue { get; set; } = string.Empty;
    }
}
