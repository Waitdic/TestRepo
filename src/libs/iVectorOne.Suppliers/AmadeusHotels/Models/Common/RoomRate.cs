namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomRate
    {
        [XmlAttribute("BookingCode")]
        public string BookingCode { get; set; } = string.Empty;

        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;

        [XmlAttribute("RoomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;

        [XmlAttribute("AvailabilityStatus")]
        public string AvailabilityStatus { get; set; } = string.Empty;

        [XmlArray("Rates")]
        [XmlArrayItem("Rate")]
        public Rate[] Rates { get; set; } = Array.Empty<Rate>();

        public Total Total { get; set; } = new();

        public RateDescription RoomRateDescription { get; set; } = new();
        
        [XmlAttribute]
        public int NumberOfUnits { get; set; }
    }
}
