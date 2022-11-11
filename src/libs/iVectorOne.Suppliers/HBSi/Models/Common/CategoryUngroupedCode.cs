namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class CategoryUngroupedCode
    {
        public CategoryUngroupedCode() { }

        [XmlAttribute("Code")]
        public string Code { get; set; } = string.Empty;
    }


}
