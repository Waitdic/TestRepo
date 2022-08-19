namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    [XmlRoot("message")]
    public class SearchRequest
    {
        [XmlElement("actionseg")]
        public string ActionSeg { get; set; } = string.Empty;

        [XmlElement("searchseg")]
        public SearchSegment SearchSeg { get; set; } = new();
    }
}
