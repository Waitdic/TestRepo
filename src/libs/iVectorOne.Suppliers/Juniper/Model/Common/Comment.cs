namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class Comment
    {
        public Comment() { }

        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }
}