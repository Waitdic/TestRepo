namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class BookingBookResponse
    {
        [XmlAttribute("bookingReference")]
        public string BookingReference { get; set; } = string.Empty;

        [XmlArray("items")]
        [XmlArrayItem("item")]
        public ItemResponse[] Items { get; set; } = Array.Empty<ItemResponse>();
    }
}
