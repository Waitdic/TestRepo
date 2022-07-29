namespace iVectorOne.Suppliers.RMI.Models
{
    using System.Xml.Serialization;

    public class SpecialOffer
    {
        [XmlElement("Name")]
        public string Name { set; get; } = string.Empty;

        [XmlElement("Total")]
        public string Total { set; get; } = string.Empty;
    }
}
