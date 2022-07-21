namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class ErrorDetail
    {
        [XmlElement("errorCode")]
        public string ErrorCode { get; set; } = string.Empty;
    }
}
