namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Warning
    {
        [XmlAttribute("ShortText")]
        public string ShortText { get; set; } = string.Empty;

        [XmlText]
        public string Text { get; set; } = string.Empty;
    }
}
