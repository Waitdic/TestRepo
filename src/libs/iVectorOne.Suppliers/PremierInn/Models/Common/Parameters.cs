namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;

    public class Parameters
    {
        [XmlAttribute]
        public string MessageType { get; set; } = string.Empty;
    }
}
