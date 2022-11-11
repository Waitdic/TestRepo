namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Comment
    {
        public Comment() { }

        [XmlElement("Text", Namespace = "http://www.opentravel.org/OTA/2003/05")]
        public string Text { get; set; } = string.Empty;
    }
}
