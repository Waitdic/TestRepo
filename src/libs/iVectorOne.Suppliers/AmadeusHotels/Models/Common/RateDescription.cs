namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RateDescription
    {
        [XmlElement("Text")]
        public string[] Text { get; set; } = Array.Empty<string>();
    }
}