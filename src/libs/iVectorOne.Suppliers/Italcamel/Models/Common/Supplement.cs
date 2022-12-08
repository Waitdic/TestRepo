namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class Supplement
    {
        [XmlElement("AMOUNT")]
        public decimal Amount { get; set; }
    }
}
