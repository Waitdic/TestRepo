namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class RoomToken
    {
        public RoomToken() { }

        [XmlAttribute("Token")]
        public string Token { get; set; } = string.Empty;

    }
}
