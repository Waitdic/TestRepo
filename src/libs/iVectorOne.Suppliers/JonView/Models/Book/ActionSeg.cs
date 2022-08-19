namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    public class ActionSeg 
    {
        [XmlElement("actioncode")]
        public string ActionCode { get; set; } = string.Empty;

        [XmlElement("status")]
        public string Status { get; set; } = string.Empty;

        [XmlElement("resnumber")]
        public string ResNumber { get; set; } = string.Empty;
    }
}
