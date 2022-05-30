namespace ThirdParty.CSSuppliers.OceanBeds.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("BookingCancellationRQ", Namespace = "http://oceanbeds.com/2014/10")]
    public class BookingCancellationRQ
    {
        public Credential? Credential { get; set; }

        public string MasterBookingId { get; set; } = string.Empty;
    }
}
