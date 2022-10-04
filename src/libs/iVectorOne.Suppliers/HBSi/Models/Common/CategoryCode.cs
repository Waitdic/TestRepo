namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class CategoryCode
    {
        public CategoryCode() { }
        [XmlAttribute("Code")]
        public string Code { get; set; } = string.Empty;
    }


}
