namespace iVectorOne.Suppliers.BedsWithEase.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Common;

    public class HotelAvailabilityResponse : SoapContent
    {
        public string SessionId { get; set; } = string.Empty;
        public string SeachId { get; set; } = string.Empty;

        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public List<Error> Errors { get; set; } = new();

        [XmlArray("Warnings")]
        [XmlArrayItem("Warning")]
        public List<Error> Warnings { get; set; } = new();

        [XmlArray("Hotels")]
        [XmlArrayItem("Hotel")]
        public List<Hotel> Hotels { get; set; } = new();

        public string PortionsSequenceId { get; set; } = string.Empty;
    }
}
