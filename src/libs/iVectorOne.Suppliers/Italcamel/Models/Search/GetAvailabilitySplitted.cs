namespace iVectorOne.Suppliers.Italcamel.Models.Search
{
    using System.Xml.Serialization;
    using Envelope;

    public class GetAvailabilitySplitted : SoapContent
    {
        [XmlElement("REQUEST")]
        public SearchRequest Request { get; set; } = new();
    }
}
