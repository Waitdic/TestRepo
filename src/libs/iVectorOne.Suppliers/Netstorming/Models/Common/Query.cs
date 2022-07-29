namespace iVectorOne.Suppliers.Netstorming.Models.Common
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;

    public class Query
    {
        [XmlElement("city")]
        public QueryCity City { get; set; } = new();

        [XmlElement("timestamp")]
        public string? Timestamp { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; } = string.Empty;

        [XmlAttribute("product")]
        public string Product { get; set; } = string.Empty;

        [XmlElement("nationality")]
        public string Nationality { get; set; } = string.Empty;

        [XmlArray("filters")]
        [XmlArrayItem("filter")]
        public string[] Filters { get; set; } = Array.Empty<string>();

        [XmlElement("checkin")]
        public QueryDate Checkin { get; set; } = new();

        [XmlElement("checkout")]
        public QueryDate Checkout { get; set; } = new();

        [XmlElement("category")]
        public QueryCategory Category { get; set; } = new();

        public bool ShouldSerializeCategory() => !string.IsNullOrEmpty(Category.Code);

        [XmlElement("hotel")]
        public QueryHotel Hotel { get; set; } = new();

        public bool ShouldSerializeHotel() => !Hotel.IsEmpty();

        [XmlArray("details")]
        [XmlArrayItem("room")]
        public Room[] Details { get; set; } = Array.Empty<Room>();

        [XmlArray("stars")]
        [XmlArrayItem("star")]
        public byte[] Stars { get; set; } = Array.Empty<byte>();

        public bool ShouldSerializeStars() => Stars.Any();

        [XmlElement("search")]
        public QuerySearch Search { get; set; } = new();

        public bool ShouldSerializeSearch() => !Search.IsEmpty();

        [XmlElement("synchronous")]
        public Synchronous Synchronous { get; set; } = new();

        public bool ShouldSerializeSynchronous() => !string.IsNullOrEmpty(Synchronous.Value);

        [XmlElement("availonly")]
        public Availonly Availonly { get; set; } = new();

        public bool ShouldSerializeAvailonly() => !string.IsNullOrEmpty(Availonly.Value);

        [XmlElement("reference")]
        public Reference Reference { get; set; } = new();

        public bool ShouldSerializeReference() => !string.IsNullOrEmpty(Reference.Code);

        [XmlArray("responses")]
        [XmlArrayItem("to")]
        public To[] To { get; set; } = Array.Empty<To>();

        public bool ShouldSerializeTo() => To.Any();
    }
}