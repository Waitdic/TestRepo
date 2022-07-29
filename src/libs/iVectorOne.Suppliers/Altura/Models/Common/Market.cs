namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class Market
    {
        public Market() { }
        [XmlElement("SourceMarket")]
        public string SourceMarket { get; set; } = string.Empty;

        [XmlElement("ClientNationality")]
        public string ClientNationality { get; set; } = string.Empty;

        public bool ShouldSerializeClientNationality() => !string.IsNullOrEmpty(ClientNationality);
    }
}
