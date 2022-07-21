namespace iVectorOne.CSSuppliers.TeamAmerica.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PriceSearchResponse : SoapContent
    {
        [XmlElement("hotelSearchResponse")]
        public HotelSearchResponse HotelSearchResponse { get; set; } = new();
    }

    public class HotelSearchResponse
    {
        [XmlElement("body")]
        public List<HotelOffer> Offers { get; set; } = new();
    }
}
