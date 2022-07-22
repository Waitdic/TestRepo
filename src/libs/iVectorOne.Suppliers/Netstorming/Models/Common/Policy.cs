namespace iVectorOne.CSSuppliers.Netstorming.Models.Common
{
    using System.Xml.Serialization;

    public class Policy
    {
        [XmlAttribute("from")]
        public string From { get; set; } = string.Empty;

        [XmlAttribute("percentage")]
        public string Percentage { get; set; } = string.Empty;
    }
}