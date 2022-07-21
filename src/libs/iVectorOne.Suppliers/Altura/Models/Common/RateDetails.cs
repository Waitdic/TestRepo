namespace ThirdParty.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class RateDetails
    {
        [XmlAttribute("NoRefundable")]
        public string NoRefundable { get; set; } = string.Empty;

        [XmlAttribute("TotalPrice")]
        public string TotalPrice { get; set; } = string.Empty;
    }
}
