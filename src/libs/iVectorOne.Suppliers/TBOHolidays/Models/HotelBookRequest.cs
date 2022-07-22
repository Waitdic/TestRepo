namespace iVectorOne.CSSuppliers.TBOHolidays.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class HotelBookRequest : SoapContent
    {
        public string ClientReferenceNumber { get; set; } = string.Empty;

        public string GuestNationality { get; set; } = string.Empty;

        [XmlArray("Guests")]
        [XmlArrayItem("Guest")]
        public Guest[] Guests { get; set; } = Array.Empty<Guest>();

        public AddressInfo AddressInfo { get; set; } = new();

        public PaymentInfo PaymentInfo { get; set; } = new();

        [XmlArray("HotelRooms")]
        [XmlArrayItem("HotelRoom")]
        public HotelRoom[] HotelRooms { get; set; } = Array.Empty<HotelRoom>();

        public string SessionId { get; set; } = string.Empty;

        public int NoOfRooms { get; set; }

        public int ResultIndex { get; set; }

        public string HotelCode { get; set; } = string.Empty;

        public string HotelName { get; set; } = string.Empty;

        [XmlElement("SpecialRequests")]
        public SpecialRequests[] SpecialRequests { get; set; } = Array.Empty<SpecialRequests>();
    }
}
