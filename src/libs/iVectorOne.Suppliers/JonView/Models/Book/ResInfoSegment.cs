namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    public class ResInfoSegment 
    {
        [XmlElement("refitem")]
        public string RefItem { get; set; } = string.Empty;

        [XmlElement("attitem")]
        public string AttItem { get; set; } = string.Empty;

        [XmlElement("resitem")]
        public string ResItem { get; set; } = string.Empty;
    }
}
