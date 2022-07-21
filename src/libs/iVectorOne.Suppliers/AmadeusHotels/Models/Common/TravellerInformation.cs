namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class TravellerInformation
    {
        [XmlElement("traveller")]
        public Traveller Traveller { get; set; } = new();

        [XmlElement("passenger")]
        public Passenger Passenger { get; set; } = new();
    }
}
