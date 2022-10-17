namespace iVectorOne.Suppliers.Italcamel.Models.Search
{
    using System.Xml.Serialization;
    using Envelope;

    public class GetAvailabilityResponse : SoapContent
    {
        [XmlElement("GETAVAILABILITYResult")]
        public SearchResponse GetAvaibilityResult { get; set; } = new();
    }
}
