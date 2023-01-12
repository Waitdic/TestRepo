namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;

    public class PaymentCard
    {
        [XmlAttribute]
        public string CardType { get; set; } = string.Empty;

        [XmlAttribute]
        public string CardNumber { get; set; } = string.Empty;

        [XmlAttribute]
        public string ExpiryDate { get; set; } = string.Empty;

        public string CardHolderName { get; set; } = string.Empty;
    }
}
