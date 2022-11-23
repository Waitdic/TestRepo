namespace iVectorOne.Suppliers.Italcamel.Models.Cancel
{
    using System.Xml.Serialization;

    public class PackageDeleteResult
    {
        [XmlElement("ERRORCODE")]
        public string ErrorCode { get; set; } = string.Empty;

        [XmlElement("STATUS")]
        public string Status { get; set; } = string.Empty;
    }
}
