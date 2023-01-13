namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class Discount
    {
        [XmlElement("AMOUNT")]
        public decimal Amount { get; set; }
    }
}
