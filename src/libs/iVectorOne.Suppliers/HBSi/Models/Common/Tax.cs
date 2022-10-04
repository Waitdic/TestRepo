namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Tax
    {
        [XmlAttribute("Type")]
        public string TaxType { get; set; } = string.Empty;

        [XmlAttribute("Amount")]
        public string Amount { get; set; } = string.Empty;
    }
}
