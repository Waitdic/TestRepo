namespace iVectorOne.Suppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class QueryCity
    {
        [XmlAttribute("code")]
        public string Code { get; set; } = string.Empty;
    }
}