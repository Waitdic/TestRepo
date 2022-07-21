namespace ThirdParty.CSSuppliers.Miki.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class HotelSearchResponse : SoapContent
    {
        [XmlArray("hotels")]
        [XmlArrayItem("hotel")]
        public Hotel[] Hotels { get; set; } = Array.Empty<Hotel>();

        [XmlArray("errors")]
        [XmlArrayItem("error")]
        public string[] Errors { get; set; } = Array.Empty<string>();

        [XmlElement("responseAuditInfo")]
        public ResponseAuditInfo ResponseAuditInfo { get; set; } = new();
    }
}
