namespace iVectorOne.Suppliers.Italcamel.Models.Search
{
    using System;
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Common;

    public class SearchRequest
    {
        [XmlElement("USERNAME")]
        public string Username { get; set; } = string.Empty;

        [XmlElement("PASSWORD")]
        public string Password { get; set; } = string.Empty;

        [XmlElement("LANGUAGEUID")]
        public string LanguageUID { get; set; } = string.Empty;

        [XmlArray("ACCOMMODATIONS")]
        [XmlArrayItem("ACCOMMODATION")]
        public string[] Accommodations { get; set; } = Array.Empty<string>();
        public bool ShouldSerializeAccommodations() => Accommodations.Length != 0;

        [XmlElement("CHECKIN")]
        public string CheckIn { get; set; } = string.Empty;

        [XmlElement("CHECKOUT")]
        public string CheckOut { get; set; } = string.Empty;

        [XmlArray("ROOMS")]
        [XmlArrayItem("ROOM")]
        public SearchRoom[] Rooms { get; set; } = Array.Empty<SearchRoom>();

        [XmlElement("INCLUDE_PRICE_BREAKDOWN")]
        public bool IncludePriceBreakdown { get; set; }
    }
}
