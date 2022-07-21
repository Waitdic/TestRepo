namespace ThirdParty.CSSuppliers.TeamAmerica.Models
{
    using System.Xml.Serialization;

    public class CancelReservation : SoapContent
    {
        [XmlElement("UserName")]
        public string UserName { get; set; } = string.Empty;

        [XmlElement("Password")]
        public string Password { get; set; } = string.Empty;

        [XmlElement("ReservationNumber")]
        public string ReservationNumber { get; set; } = string.Empty;
    }
}
