namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class TpaExtensions
    {
        public TpaExtensions() { }

        [XmlArray(ElementName = "Providers")]
        [XmlArrayItem(ElementName = "Provider")]
        public List<Provider> Providers { get; set; } = new List<Provider>();

        [XmlArray(ElementName = "ProviderTokens")]
        [XmlArrayItem(ElementName = "Token")]
        public List<Token> ProviderTokens { get; set; } = new List<Token>();

        [XmlElement("RoomToken")]
        public RoomToken RoomToken { get; set; } = new RoomToken();


        [XmlElement("ProviderID")]
        public ProviderID ProviderID { get; set; } = new ProviderID();

        [XmlElement("HotelInfo")]
        public HotelInfo HotelInfo { get; set; } = new HotelInfo();

        [XmlArray("Photos")]
        [XmlArrayItem("Photo")]
        public List<Photo> Photos { get; set; } = new List<Photo>();
    }

    public class Photo
    {
        public Photo() { }

        [XmlAttribute("URL")]
        public string Url { get; set; } = string.Empty;
    }

    public class RoomToken
    {
        public RoomToken() { }

        [XmlAttribute("Token")]
        public string Token { get; set; } = string.Empty;

    }

    public class ProviderID
    {
        public ProviderID() { }

        [XmlAttribute("Provider")]
        public string Provider { get; set; } = string.Empty;
    }
}
