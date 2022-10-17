namespace iVectorOne.Suppliers.Italcamel.Models.Search
{
    using System;
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Common;

    public class SearchRequest : Request
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

        [XmlArray("ROOMS")]
        [XmlArrayItem("ROOM")]
        public SearchRoom[] Rooms { get; set; } = Array.Empty<SearchRoom>();
    }
}
