namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class SpecialRequest 
    {
        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }
}
