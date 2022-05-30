using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Comment")]
    public class Comment
    {
        public Comment(string type, string text)
        {
            Type = type;
            Text = text;
        }

        public Comment()
        {
        }

        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}
