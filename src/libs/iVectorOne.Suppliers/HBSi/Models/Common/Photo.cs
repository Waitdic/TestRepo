namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Photo
    {
        public Photo() { }

        [XmlAttribute("URL")]
        public string Url { get; set; } = string.Empty;
    }
}
