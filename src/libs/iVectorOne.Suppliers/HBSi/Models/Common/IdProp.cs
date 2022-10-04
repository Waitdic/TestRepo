namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class IdProp
    {
        [XmlAttribute("ID")]
        public string Id { get; set; } = string.Empty;
    }


}
