namespace iVectorOne.Suppliers.JonView.Models.Prebook
{
    using System.Xml.Serialization;

    [XmlRoot("message")]
    public class CancellationPolicyRequest : IMessageRq
    {
        [XmlElement("actionseg")]
        public string ActionSeg { get; set; } = string.Empty;

        [XmlElement("searchseg")]
        public PrebookSearchSegment SearchSeg { get; set; } = new();
    }
}
