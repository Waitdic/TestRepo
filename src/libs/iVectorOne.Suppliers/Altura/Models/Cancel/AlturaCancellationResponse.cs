namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Xml.Serialization;

    [XmlRoot("AlturaDS_responses")]
    public class AlturaCancellationResponse
    {
        public AlturaCancellationResponse() { }

        [XmlElement("Response")]
        public CancellationResponse Response { get; set; } = new();

    }
}
