namespace iVectorOne.CSSuppliers.TeamAmerica.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomItem
    {
        [XmlElement("ProductCode")]
        public string ProductCode { get; set; } = string.Empty;

        [XmlElement("ProductDate")]
        public string ProductDate { get; set; } = string.Empty;

        [XmlElement("Occupancy")]
        public string Occupancy { get; set; } = string.Empty;

        [XmlElement("NumberOfNights")]
        public int NumberOfNights { get; set; }

        [XmlElement("Language")]
        public string Language { get; set; } = Constant.ENG;

        [XmlElement("Quantity")]
        public int Quantity { get; set; } = 1;

        [XmlElement("ItemRemarks")]
        public string ItemRemarks { get; set; } = string.Empty;

        [XmlElement("RateExpected")]
        public string RateExpected { get; set; } = string.Empty;

        [XmlArray("Passengers")]
        [XmlArrayItem("NewPassenger")]
        public List<NewPassenger> Passengers { get; set; } = new();
    }
}
