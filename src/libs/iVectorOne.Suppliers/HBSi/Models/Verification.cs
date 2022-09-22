namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Verification
    {
        [XmlElement("PersonName")]
        public PersonName PersonName { get; set; } = new();
    }
}
