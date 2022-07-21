namespace ThirdParty.CSSuppliers.Hotelston.Models.Common
{
    using System.Xml.Serialization;

    public abstract class RequestBase : SoapContent
    {
        [XmlElement("locale")]
        public string Locale { get; set; } = string.Empty;

        [XmlElement("loginDetails")]
        public LoginDetails LoginDetails { get; set; } = new();

        [XmlElement("currency")]
        public string Currency { get; set; } = string.Empty;

        [XmlElement("netRates")]
        public string NetRates { get; set; } = string.Empty;
    }
}
