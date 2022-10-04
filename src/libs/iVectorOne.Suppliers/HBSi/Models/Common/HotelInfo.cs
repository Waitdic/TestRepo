namespace iVectorOne.Suppliers.HBSi.Models
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
}
