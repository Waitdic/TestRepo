namespace ThirdParty.CSSuppliers.TeamAmerica.Models
{
    using System.Xml.Serialization;

    public class NewMultiItemReservationResponse : SoapContent
    {
        [XmlElement("newMultiItemReservationResponse")]
        public ReservationResponse ReservationResponse { get; set; } = new();
    }

    public class ReservationResponse
    {
        [XmlElement("ReservationInformation")]
        public ReservationInformation ReservationInformation { get; set; } = new();
    }
}
