namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class CategoryName
    {
        public CategoryName() { }

        [XmlAttribute("Name")]

        public string Name { get; set; } = string.Empty;
    }


}
