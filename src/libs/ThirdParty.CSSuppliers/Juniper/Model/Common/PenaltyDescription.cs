namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class PenaltyDescription
    {
        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }
}