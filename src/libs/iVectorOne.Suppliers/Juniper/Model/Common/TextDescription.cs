namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class TextDescription
    {
        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }
}