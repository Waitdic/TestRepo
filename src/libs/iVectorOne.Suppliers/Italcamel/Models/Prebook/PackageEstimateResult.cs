namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using System.Xml.Serialization;

    public class PackageEstimateResult
    {
        [XmlElement("ERRORCODE")]
        public int ErrorCode { get; set; }

        [XmlElement("STATUS")]
        public int Status { get; set; }

        [XmlElement("PACKAGE")]
        public Package Package { get; set; } = new();
    }
}
