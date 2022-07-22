namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class Discount
    {
        [XmlAttribute("Discount")]
        public decimal DiscountValue { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
