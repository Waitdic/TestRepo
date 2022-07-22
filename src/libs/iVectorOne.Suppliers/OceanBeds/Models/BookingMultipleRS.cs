namespace iVectorOne.CSSuppliers.OceanBeds.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("BookingMultipleRS", Namespace = "http://oceanbeds.com/2014/10")]
    public class BookingMultipleRS
    {
        public string Response { get; set; } = string.Empty;

        public Status Status { get; set; } = new();

        [XmlArray("InvalidBookingList")]
        [XmlArrayItem("Booking")]
        public Booking[] InvalidBookingList { get; set; } = Array.Empty<Booking>();

    }
}
