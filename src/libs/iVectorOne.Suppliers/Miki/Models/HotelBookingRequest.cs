namespace iVectorOne.CSSuppliers.Miki.Models
{
    using System.Xml.Serialization;
    using Common;

    public class HotelBookingRequest : SoapContent
    {
        [XmlAttribute("versionNumber")]
        public string VersionNumber { get; set; } = string.Empty;

        [XmlElement("requestAuditInfo")]
        public RequestAuditInfo RequestAuditInfo { get; set; } = new();

        [XmlElement("booking")]
        public BookingBookRequest Booking { get; set; } = new();
    }
}
