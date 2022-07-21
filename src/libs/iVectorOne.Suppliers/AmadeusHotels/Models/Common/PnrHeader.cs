namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class PnrHeader
    {
        [XmlElement("reservationInfo")]
        public ReservationInfo ReservationInfo { get; set; } = new();
    }
}
