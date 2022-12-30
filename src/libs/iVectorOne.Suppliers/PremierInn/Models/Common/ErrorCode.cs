namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;

    public class ErrorCode
    {
        [XmlAttribute]
        public string Status { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}
