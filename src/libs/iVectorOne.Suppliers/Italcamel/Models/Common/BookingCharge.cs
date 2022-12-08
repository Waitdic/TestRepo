namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class BookingCharge
    {
        [XmlElement("CHARGEDATE")]
        public DateTime ChargeDate { get; set; }

        [XmlElement("ROOMUID")]
        public string RoomUID { get; set; } = string.Empty;

        [XmlElement("CHARGEAMOUNT")]
        public decimal ChargeAmount { get; set; }

        [XmlElement("CURRENCY")]
        public string Currency { get; set; } = string.Empty;
    }
}
