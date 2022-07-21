namespace ThirdParty.CSSuppliers.Hotelston.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("BookingTermsResponse")]
    public class BookingTermsResponse : SoapContent
    {
        [XmlElement("success")]
        public bool Success { get; set; }

        [XmlElement("bookingTerms")]
        public BookingTerms[] BookingTerms { get; set; } = Array.Empty<BookingTerms>();
    }
}
