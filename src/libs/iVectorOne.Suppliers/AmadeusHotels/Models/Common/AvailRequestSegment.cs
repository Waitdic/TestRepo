namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class AvailRequestSegment
    {
        [XmlAttribute("InfoSource")]
        public string InfoSource { get; set; } = string.Empty;
        public bool ShouldSerializeInfoSource() => InfoSource != string.Empty;

        public HotelSearchCriteria HotelSearchCriteria { get; set; } = new();

        public string MoreDataEchoToken { get; set; } = string.Empty;
        public bool ShouldSerializeMoreDataEchoToken() => MoreDataEchoToken != string.Empty;
    }
}
