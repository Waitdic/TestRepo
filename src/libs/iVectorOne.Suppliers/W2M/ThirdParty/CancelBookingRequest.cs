using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618

    [XmlRoot(ElementName = "CancelBooking")]
    public class CancelBookingRequest
    {
        public CancelBookingRequest(CancellationRequest cancellationRequest)
        {
            CancellationRequest = cancellationRequest;
        }

        public CancelBookingRequest()
        {
        }

        [XmlElement(ElementName = "CancelRQ")]
        public CancellationRequest CancellationRequest { get; set; }
    }
}
