using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "HotelOffers")]
    public class HotelOffers
    {
        [XmlElement(ElementName = "HotelOffer")]
        public HotelOffer HotelOffer { get; set; }
    }
}
