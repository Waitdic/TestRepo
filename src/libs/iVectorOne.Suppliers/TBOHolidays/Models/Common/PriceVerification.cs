namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class PriceVerification
    {
        [XmlAttribute("Status")]
        public StatusEnum Status { get; set; }

        [XmlAttribute("PriceChanged")]
        public bool PriceChanged { get; set; }

        [XmlAttribute("AvailableOnNewPrice")]
        public bool AvailableOnNewPrice { get; set; }

        [XmlArray("HotelRooms")]
        [XmlArrayItem("HotelRoom")]
        public HotelRoom[] HotelRooms { get; set; } = Array.Empty<HotelRoom>();
    }
}
