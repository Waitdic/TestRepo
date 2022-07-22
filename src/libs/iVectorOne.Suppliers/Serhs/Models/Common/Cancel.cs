namespace iVectorOne.CSSuppliers.Serhs.Models.Common
{
    using System.Xml.Serialization;

    public class Cancel
    {
        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;
    }
}
