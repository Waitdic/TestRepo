namespace iVectorOne.Suppliers.AmadeusHotels.Models
{
    using System.Xml.Serialization;
    using Common;
    using Soap;

    public class OTAHotelAvailRQ : SoapContent
    {
        [XmlAttribute("RateRangeOnly")]
        public bool RateRangeOnly { get; set; }

        [XmlAttribute("RateDetailsInd")]
        public bool RateDetailsInd { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlAttribute("EchoToken")]
        public string EchoToken { get; set; } = string.Empty;

        [XmlAttribute("AvailRatesOnly")]
        public bool AvailRatesOnly { get; set; }

        [XmlAttribute("SummaryOnly")]
        public bool SummaryOnly { get; set; }

        [XmlAttribute("SortOrder")]
        public string SortOrder { get; set; } = string.Empty;

        [XmlAttribute("SearchCacheLevel")]
        public string SearchCacheLevel { get; set; } = string.Empty;
        public bool ShouldSerializeSearchCacheLevel() => !string.IsNullOrEmpty(SearchCacheLevel);

        [XmlAttribute("PrimaryLangID")]
        public string PrimaryLangID { get; set; } = string.Empty;
        public bool ShouldSerializePrimaryLangID() => !string.IsNullOrEmpty(PrimaryLangID);

        [XmlElement("AvailRequestSegments", Namespace = "http://www.opentravel.org/OTA/2003/05")]
        public AvailRequestSegments AvailRequestSegments { get; set; } = new();
    }
}
