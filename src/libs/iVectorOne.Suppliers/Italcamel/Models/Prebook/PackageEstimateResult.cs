namespace iVectorOne.Suppliers.Italcamel.Models.Prebook
{
    using System.Xml.Serialization;

    public class PackageEstimateResult
    {
        [XmlElement("STATUS")]
        public int Status { get; set; }

        [XmlElement("ERRORCODE")]
        public string ErrorCode { get; set; } = string.Empty;

        [XmlElement("PACKAGE")]
        public Package Package { get; set; } = new();
    }
}
