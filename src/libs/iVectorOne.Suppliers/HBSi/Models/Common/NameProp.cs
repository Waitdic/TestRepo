namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class NameProp
    {
        public NameProp() { }
        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;
    }


}
