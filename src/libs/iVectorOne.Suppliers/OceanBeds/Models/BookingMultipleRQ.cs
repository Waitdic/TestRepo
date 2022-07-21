namespace ThirdParty.CSSuppliers.OceanBeds.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("BookingMultipleRQ", Namespace = "http://oceanbeds.com/2014/10")]
    public class BookingMultipleRQ
    {
        public Credential Credential { get; set; } = new();
        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string BookingReference { get; set; } = string.Empty;

        [XmlArray("BookingList")]
        [XmlArrayItem("Booking")]
        public List<Booking> BookingList { get; set; } = new();

    }
}
