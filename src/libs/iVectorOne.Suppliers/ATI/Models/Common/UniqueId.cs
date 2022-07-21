namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class UniqueId
    {
        [XmlAttribute("ID")]
        public string ID { get; set; } = string.Empty;
    }
}
