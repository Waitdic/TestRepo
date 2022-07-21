namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class Supplement
    {
        [XmlAttribute("SuppChargeType")]
        public SuppChargeType SuppChargeType { get; set; }

        [XmlAttribute("SuppIsMandatory")]
        public bool SuppIsMandatory { get; set; }

        [XmlAttribute("SuppID")]
        public int SuppID { get; set; }

        [XmlAttribute("Price")]
        public decimal Price { get; set; }
    }
}
