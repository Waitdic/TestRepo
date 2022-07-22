using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Reservations")]
    public class Reservations
    {
        [XmlElement(ElementName = "Reservation")]
        public Reservation Reservation { get; set; }
    }
}
