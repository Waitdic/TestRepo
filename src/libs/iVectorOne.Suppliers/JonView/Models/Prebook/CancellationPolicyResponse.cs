namespace iVectorOne.Suppliers.JonView.Models.Prebook
{
    using System.Xml.Serialization;

    [XmlRoot("message")]
    public class CancellationPolicyResponse : IMessageRs
    {
        [XmlElement("actionseg")]
        public ActionSeg ActionSeg { get; set; } = new();

        [XmlElement("productlistseg")]
        public ProductListSeg ProductListSeg { get; set; } = new();
    }
}
