namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Hotel
    {
        [XmlElement("productCode")]
        public string ProductCode { get; set; } = string.Empty;

        [XmlElement("currencyCode")]
        public string CurrencyCode { get; set; } = string.Empty;

        [XmlArray("roomOptions")]
        [XmlArrayItem("roomOption")]
        public RoomOption[] RoomOptions { get; set; } = Array.Empty<RoomOption>();
    }
}
