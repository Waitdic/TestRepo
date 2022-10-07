namespace iVectorOne.Suppliers.Italcamel.Models.Cancel
{
    using System.Xml.Serialization;

    public class CancelResponse
    {
        [XmlElement("ERRORCODE")]
        public string ErrorCode { get; set; } = string.Empty;
    }
}
