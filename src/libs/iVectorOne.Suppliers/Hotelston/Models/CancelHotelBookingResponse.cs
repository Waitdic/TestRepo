namespace ThirdParty.CSSuppliers.Hotelston.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("CancelHotelBookingResponse")]
    public class CancelHotelBookingResponse : SoapContent
    {
        [XmlElement("success")]
        public bool Success { get; set; }

        [XmlElement("cancellationFee")]
        public decimal CancellationFee { get; set; }
    }
}
