namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class Base
    {
        [XmlAttribute("AmountAfterTax")]
        public decimal AmountAfterTax { get; set; }
    }
}
