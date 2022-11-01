namespace iVectorOne.Suppliers.Italcamel.Models.Cancel
{
    using System;
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Common;
    using iVectorOne.Suppliers.Italcamel.Models.Envelope;

    public class GetBookingChargeResponse : SoapContent
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
