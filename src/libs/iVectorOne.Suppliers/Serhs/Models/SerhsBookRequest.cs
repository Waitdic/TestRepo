using System;
using System.Collections.Generic;

namespace iVectorOne.Suppliers.Serhs.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("request")]
    public class SerhsBookRequest : BaseRequest
    {
        public SerhsBookRequest() { }

        public SerhsBookRequest(string version, string clientCode, string password, string branch, string tradingGroup,
            string languageCode) : base(version, clientCode, password, branch, tradingGroup, languageCode) { }

        [XmlAttribute("type")]
        public override string Type { get; set; } = "CONFIRM";

        [XmlElement("customer")]
        public Customer Customer { get; set; } = new();

        [XmlArray("preBookings")]
        [XmlArrayItem("preBooking")]
        public Booking[] PreBookings { get; set; } = Array.Empty<Booking>();
    }
}
