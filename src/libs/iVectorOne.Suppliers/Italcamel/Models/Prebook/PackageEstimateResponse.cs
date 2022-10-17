namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;

    public class PackageEstimateResponse : SoapContent
    {
        [XmlElement("ERRORCODE")]
        public int ErrorCode { get; set; }

        [XmlElement("STATUS")]
        public int Status { get; set; }

        [XmlElement("PACKAGE")]
        public Package Package { get; set; } = new();
    }
}
