namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class PosSource
    {
        [XmlElement("RequestorID")]
        public RequestorId RequestorID { get; set; } = new();

        [XmlElement("BookingChannel")]
        public BookingChannel BookingChannel { get; set; } = new();
    }
}
