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
        public string LanguageuId { get; set; } = string.Empty;

        [XmlElement("ACCOMMODATIONUID")]
        public string AccomodationuId { get; set; } = string.Empty;
        public bool ShouldSerializeAccomodationuId() => !string.IsNullOrEmpty(AccomodationuId);

        [XmlElement("MACROREGIONUID")]
        public string MacroregionuId { get; set; } = string.Empty;
        public bool ShouldSerializeMacroregionuId() => !string.IsNullOrEmpty(MacroregionuId);

        [XmlElement("CITYUID")]
        public string CityuId { get; set; } = string.Empty;
        public bool ShouldSerializeCityuId() => !string.IsNullOrEmpty(CityuId);

        [XmlElement("CHECKIN")]
        public string CheckIn { get; set; } = string.Empty;

        [XmlElement("CHECKOUT")]
        public string CheckOut { get; set; } = string.Empty;

        [XmlArray("ROOMS")]
        [XmlArrayItem("ROOM")]
        public Room[] Rooms { get; set; } = Array.Empty<Room>();
    }
}
