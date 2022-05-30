namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class TextDescription
    {
        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }
}
