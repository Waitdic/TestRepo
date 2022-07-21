namespace iVectorOne.CSSuppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class Remark
    {
        [XmlAttribute("text")]
        public string Text { get; set; } = string.Empty;

        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;
    }
}