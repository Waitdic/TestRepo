using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Reservations")]
    public class Reservations
    {
        [XmlElement(ElementName = "Reservation")]
        public Reservation Reservation { get; set; }
    }
}
