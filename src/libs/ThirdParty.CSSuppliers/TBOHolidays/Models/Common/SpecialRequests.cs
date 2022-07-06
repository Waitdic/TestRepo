namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class SpecialRequests
    {
        [XmlAttribute("RequestId")]
        public int RequestId { get; set; }

        [XmlAttribute("RequestType")]
        public string RequestType { get; set; } = string.Empty;

        [XmlAttribute("Remarks")]
        public string Remarks { get; set; } = string.Empty;
    }
}
