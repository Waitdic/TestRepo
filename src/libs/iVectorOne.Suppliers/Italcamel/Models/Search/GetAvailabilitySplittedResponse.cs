namespace iVectorOne.Suppliers.Italcamel.Models.Search
{
    using System.Xml.Serialization;
    using Envelope;

    public class GetAvailabilitySplittedResponse : SoapContent
    {
        [XmlElement("RESPONSE")]
        public SearchResponse Response { get; set; } = new();
    }
}
