namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Common;

    public class Package
    {
        [XmlElement("STATUS")]
        public int Status { get; set; }

        [XmlElement("BOOKING")]
        public Booking Booking { get; set; } = new();
    }
}
