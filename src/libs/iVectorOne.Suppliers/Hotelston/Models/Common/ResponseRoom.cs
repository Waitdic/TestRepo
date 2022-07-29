namespace iVectorOne.Suppliers.Hotelston.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class ResponseRoom
    {
        [XmlAttribute("seqNo")]
        public int SeqNo { get; set; }

        [XmlElement("roomType")]
        public RoomType RoomType { get; set; } = new();

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("boardType")]
        public BoardType BoardType { get; set; } = new();

        [XmlAttribute("price")]
        public decimal Price { get; set; }

        [XmlAttribute("specialOffer")]
        public bool SpecialOffer { get; set; }

        [XmlElement("specifficSpecialOffer")]
        public SpecifficSpecialOffer[] SpecifficSpecialOffers { get; set; } = Array.Empty<SpecifficSpecialOffer>();

        [XmlElement("bookingTerms")]
        public BookingTerms BookingTerms { get; set; } = new();
    }
}