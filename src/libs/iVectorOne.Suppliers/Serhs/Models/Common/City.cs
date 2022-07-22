namespace iVectorOne.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class City
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("zipCode")]
        public string? ZipCode { get; set; }
    }
}