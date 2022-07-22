namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class TravellerInfo
    {
        [XmlElement("elementManagementPassenger")]
        public ElementManagementPassenger ElementManagementPassenger { get; set; } = new();

        [XmlElement("passengerData")]
        public PassengerData PassengerData { get; set; } = new();
    }
}
