namespace iVectorOne.Suppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class HotelInfo
    {
        public HotelInfo() { }

        [XmlElement("CategoryCode")]
        public CategoryCode CategoryCode { get; set; } = new CategoryCode();

        [XmlElement("CategoryUngroupedCode")]
        public CategoryUngroupedCode CategoryUngroupedCode { get; set; } = new CategoryUngroupedCode();

        [XmlElement("CategoryName")]
        public CategoryName CategoryName { get; set; } = new CategoryName();

        [XmlElement("Id")]
        public IdProp Id { get; set; } = new IdProp();

        [XmlElement("Name")]
        public NameProp Name { get; set; } = new NameProp();

        [XmlElement("Type")]
        public TypeProp TypeProp { get; set; } = new TypeProp();
    }

    public class CategoryCode
    {
        public CategoryCode() { }
        [XmlAttribute("Code")]
        public string Code { get; set; } = string.Empty;
    }

    public class CategoryUngroupedCode
    {
        public CategoryUngroupedCode() { }

        [XmlAttribute("Code")]
        public string Code { get; set; } = string.Empty;
    }

    public class CategoryName
    {
        public CategoryName() { }

        [XmlAttribute("Name")]

        public string Name { get; set; } = string.Empty;
    }

    public class IdProp
    {
        [XmlAttribute("ID")]
        public string Id { get; set; } = string.Empty;
    }

    public class NameProp
    {
        public NameProp() { }
        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;
    }

    public class TypeProp
    {
        public TypeProp() { }

        [XmlAttribute("Type")]
        public string TypePropAttr { get; set; } = string.Empty;
    }


}
