namespace iVectorOne.CSSuppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    public class SuppInfo
    {
        [XmlAttribute("SuppID")]
        public int SuppID { get; set; }

        [XmlAttribute("SuppChargeType")]
        public SuppChargeType SuppChargeType { get; set; }

        [XmlAttribute("Price")]
        public decimal Price { get; set; }

        [XmlAttribute("SuppIsSelected")]
        public bool SuppIsSelected { get; set; }
    }
}
