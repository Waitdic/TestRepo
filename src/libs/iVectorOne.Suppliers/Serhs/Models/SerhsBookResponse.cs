namespace ThirdParty.CSSuppliers.Serhs.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("response")]
    public class SerhsBookResponse
    {
        [XmlArray("bookings")]
        [XmlArrayItem("booking")]
        public List<Booking> Bookings { get; set; } = new();
    }
}
