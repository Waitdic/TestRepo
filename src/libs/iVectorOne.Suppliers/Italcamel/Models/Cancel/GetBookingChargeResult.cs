namespace iVectorOne.Suppliers.Italcamel.Models.Cancel
{
    using iVectorOne.Suppliers.Italcamel.Models.Common;
    using System.Xml.Serialization;
    using System;

    public class GetBookingChargeResult
    {
        [XmlArray("BOOKINGCHARGES")]
        [XmlArrayItem("BOOKINGCHARGE")]
        public BookingCharge[] BookingCharges { get; set; } = Array.Empty<BookingCharge>();

        [XmlElement("STATUS")]
        public string? Status { get; set; }

        [XmlElement("ERRORMESSAGE")]
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
