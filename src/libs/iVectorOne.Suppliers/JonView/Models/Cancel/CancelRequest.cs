namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    [XmlRoot("message")]
    public class CancelRequest : IMessageRq
    {
        [XmlElement("actionseg")]
        public string ActionSeg { get; set; } = string.Empty;

        [XmlElement("resinfoseg")]
        public ResInfoCancel ResInfo { get; set; } = new();
    }
}
