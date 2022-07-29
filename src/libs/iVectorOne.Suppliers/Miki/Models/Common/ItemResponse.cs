namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class ItemResponse
    {
        [XmlAttribute("tourReference")]
        public string TourReference { get; set; } = string.Empty;

        [XmlElement("hotel")]
        public BookingHotel Hotel { get; set; } = new();
    }
}
