namespace iVectorOne.CSSuppliers.Netstorming.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Hotel
    {
        [XmlElement("agreement")]
        public Agreement[] Agreement { get; set; } = Array.Empty<Agreement>();

        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;

        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("stars")]
        public string Stars { get; set; } = string.Empty;

        [XmlAttribute("location")]
        public string Location { get; set; } = string.Empty;

        [XmlAttribute("address")]
        public string Address { get; set; } = string.Empty;

        [XmlAttribute("promo")]
        public string Promo { get; set; } = string.Empty;

        [XmlAttribute("city")]
        public string City { get; set; } = string.Empty;
    }
}