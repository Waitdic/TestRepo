namespace ThirdParty.CSSuppliers.Hotelston.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Hotel
    {
        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("room")]
        public ResponseRoom[] Rooms { get; set; } = Array.Empty<ResponseRoom>();
    }
}