namespace iVectorOne.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;
    public class Customer
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("surname")]
        public string? Surname { get; set; }
    }
}