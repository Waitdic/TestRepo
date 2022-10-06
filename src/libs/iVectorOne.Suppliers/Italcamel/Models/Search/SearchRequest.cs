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
        public string LanguageuId { get; set; } = string.Empty;

        [XmlElement("MACROREGIONUID")]
        public string MacroregionuId { get; set; } = string.Empty;
        public bool ShouldSerializeMacroregionuId() => !string.IsNullOrEmpty(MacroregionuId);

        [XmlElement("CITYUID")]
        public string CityuId { get; set; } = string.Empty;
        public bool ShouldSerializeCityuId() => !string.IsNullOrEmpty(CityuId);

        [XmlArray("ROOMS")]
        [XmlArrayItem("ROOM")]
        public SearchRoom[] Rooms { get; set; } = Array.Empty<SearchRoom>();
    }
}
