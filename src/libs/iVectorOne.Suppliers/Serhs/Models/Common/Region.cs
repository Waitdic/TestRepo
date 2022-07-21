namespace iVectorOne.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Region
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }
    }
}
