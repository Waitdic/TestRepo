namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class OTA_HotelResV2Service
    {
        public OTA_HotelResV2Service() { }

        [XmlElement("OTA_HotelResRQ")]
        public HotelReservationRequest HotelReservationRequest { get; set; } = new();

    }
}
