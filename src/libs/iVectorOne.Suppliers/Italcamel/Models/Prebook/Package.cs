namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Common;

    public class Package
    {
        [XmlElement("UID")]
        public string UID { get; set; } = string.Empty;
        
        [XmlElement("STATUS")]
        public int Status { get; set; }
        
        [XmlElement("NUMBER")]
        public string Number { get; set; } = string.Empty;

        [XmlElement("REFERENCENUMBER")]
        public string ReferenceNumber { get; set; } = string.Empty;

        [XmlElement("BOOKING")]
        public Booking Booking { get; set; } = new();
    }
}
