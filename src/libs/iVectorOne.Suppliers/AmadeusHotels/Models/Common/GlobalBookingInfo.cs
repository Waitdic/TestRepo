namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class GlobalBookingInfo
    {
        [XmlElement("markerGlobalBookingInfo")]
        public MarkerGlobalBookingInfo MarkerGlobalBookingInfo { get; set; } = new();

        [XmlElement("representativeParties")] 
        public RepresentativeParties RepresentativeParties { get; set; } = new();

        [XmlElement("bookingInfo")]
        public BookingInfo BookingInfo { get; set; } = new();
    }
}
