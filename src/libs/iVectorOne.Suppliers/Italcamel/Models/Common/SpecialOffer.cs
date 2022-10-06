namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class SpecialOffer
    {
        [XmlElement("AMOUNT")]
        public decimal Amount { get; set; }
    }
}
