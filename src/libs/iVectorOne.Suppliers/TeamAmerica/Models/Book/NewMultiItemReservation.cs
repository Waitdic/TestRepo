namespace iVectorOne.CSSuppliers.TeamAmerica.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class NewMultiItemReservation : SoapContent
    {
        [XmlElement("UserName")]
        public string UserName { get; set; } = string.Empty;

        [XmlElement("Password")]
        public string Password { get; set; } = string.Empty;

        [XmlElement("AgentName")]
        public string AgentName { get; set; } = string.Empty;

        [XmlElement("AgentEmail")]
        public string AgentEmail { get; set; } = string.Empty;

        [XmlElement("ClientReference")]
        public string ClientReference { get; set; } = string.Empty;

        [XmlArray("Items")]
        [XmlArrayItem("NewItem")]
        public List<RoomItem> RoomItems { get; set; } = new();
    }
}
