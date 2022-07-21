namespace ThirdParty.CSSuppliers.TeamAmerica.Models
{
    using System.Xml.Serialization;

    public class ReservationInformation
    {
        [XmlElement("ReservationNumber")]
        public string ReservationNumber { get; set; } = string.Empty;

        [XmlElement("ReservationStatus")]
        public string ReservationStatus { get; set; } = string.Empty;

    }
}
