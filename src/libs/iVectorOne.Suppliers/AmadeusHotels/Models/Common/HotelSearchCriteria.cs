namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class HotelSearchCriteria
    {
        [XmlAttribute("AvailableOnlyIndicator")]
        public bool AvailableOnlyIndicator { get; set; }
        public bool ShouldSerializeAvailableOnlyIndicator() => AvailableOnlyIndicator;

        public Criterion Criterion { get; set; } = new();
    }
}
