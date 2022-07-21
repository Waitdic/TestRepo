namespace iVectorOne.CSSuppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class To
    {
        [XmlAttribute("url")]
        public string Url { get; set; } = string.Empty;
    }
}