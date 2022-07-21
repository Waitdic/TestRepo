using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.TeamAmerica.Models
{
    public class CancelReservationResponse : SoapContent
    {
        [XmlElement("cancelReservationResp")]
        public CancelResponse CancelResponse { set; get; } = new();
    }

    public class CancelResponse
    {
        [XmlElement("ReservationStatusCode")]
        public string ReservationStatusCode { set; get; } = string.Empty;
    }
}
