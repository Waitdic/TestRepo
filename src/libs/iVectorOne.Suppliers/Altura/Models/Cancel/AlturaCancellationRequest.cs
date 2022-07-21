namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Xml.Serialization;

    [XmlRoot("AlturaDS_requests")]
    public class AlturaCancellationRequest
    {
        public AlturaCancellationRequest() { }

        [XmlElement("Request")]
        public CancellationRequest Request { get; set; } = new();
    }
}
