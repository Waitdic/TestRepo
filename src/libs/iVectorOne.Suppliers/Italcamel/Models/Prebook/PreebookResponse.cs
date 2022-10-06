namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Common;

    public class PreebookResponse
    {
        [XmlElement("ERRORCODE")]
        public int ErrorCode { get; set; }

        [XmlElement("STATUS")]
        public int Status { get; set; }

        [XmlElement("BOOKING")]
        public Booking Booking { get; set; } = new();
    }
}
