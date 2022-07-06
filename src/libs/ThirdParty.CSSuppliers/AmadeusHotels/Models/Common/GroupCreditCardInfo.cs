namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class GroupCreditCardInfo
    {
        [XmlElement("creditCardInfo")]
        public CreditCardInfo CreditCardInfo { get; set; } = new();

        [XmlElement("cardHolderAddress")]
        public CardHolderAddress CardHolderAddress { get; set; } = new();
    }
}
