using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "CancelRequest")]
    public class CancelRequest
    {
        public CancelRequest(string reservationLocator)
        {
            ReservationLocator = reservationLocator;
        }

        public CancelRequest()
        {
        }

        [XmlAttribute(AttributeName = "ReservationLocator")]
        public string ReservationLocator { get; set; }
    }
}
