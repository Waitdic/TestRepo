namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    [XmlRoot("message")]
    public class CancelResponse : IMessageRs
    {
        [XmlElement("actionseg")]
        public ActionSeg ActionSeg { get; set; } = new();
    }
}
