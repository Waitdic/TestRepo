namespace iVectorOne.CSSuppliers.Hotelston.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("SearchHotelsResponse")]
    public class SearchHotelsResponse : SoapContent
    {
        [XmlElement("success")]
        public bool Success { get; set; }

        [XmlElement("hotel")]
        public Hotel[] Hotels { get; set; } = Array.Empty<Hotel>();

        [XmlElement("searchId")]
        public string SearchId { get; set; } = string.Empty;
    }
}
