using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "AdditionalElements")]
    public class AdditionalElements
    {
        [XmlElement(ElementName = "HotelOffers")]
        public HotelOffers HotelOffers { get; set; }
    }
}
