namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;

    public class Session
    {
        [XmlAttribute]
        public string ID { get; set; } = string.Empty;
    }
}
