namespace iVectorOne.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class Item
    {
        [XmlAttribute("itemNumber")]
        public int ItemNumber { get; set; }

        [XmlElement("immediateConfirmationRequired")]
        public bool ImmediateConfirmationRequired { get; set; }

        [XmlElement("productCode")]
        public string ProductCode { get; set; } = string.Empty;

        [XmlElement("leadPaxName")]
        public LeadPaxName LeadPaxName { get; set; } = new();

        [XmlElement("hotel")] 
        public BookingHotel Hotel { get; set; } = new();
    }
}
