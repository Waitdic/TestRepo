namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    using System.Xml.Serialization;

    [XmlRoot("RoomRate")]
    public class RoomRate
    {
        [XmlAttribute("TotalFare")]
        public decimal TotalFare { get; set; }

        [XmlAttribute("B2CRates")]
        public bool B2CRates { get; set; }

        [XmlAttribute("AgentMarkUp")]
        public decimal AgentMarkUp { get; set; }

        [XmlAttribute("RoomTax")]
        public decimal RoomTax { get; set; }

        [XmlAttribute("RoomFare")]
        public decimal RoomFare { get; set; }

        [XmlAttribute("Currency")]
        public string Currency { get; set; } = string.Empty;
    }
}
