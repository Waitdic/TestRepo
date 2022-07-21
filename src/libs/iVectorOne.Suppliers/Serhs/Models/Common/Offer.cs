namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Offer
    {
        [XmlAttribute("code")]
        public string? Code { get; set; }
    }
}