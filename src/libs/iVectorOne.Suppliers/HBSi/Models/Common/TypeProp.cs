namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class TypeProp
    {
        public TypeProp() { }

        [XmlAttribute("Type")]
        public string TypePropAttr { get; set; } = string.Empty;
    }
}
