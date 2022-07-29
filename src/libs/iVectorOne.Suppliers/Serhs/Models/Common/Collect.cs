namespace iVectorOne.Suppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Collect
    {
        [XmlElement("address")]
        public string? Address { get; set; }
    }
}
