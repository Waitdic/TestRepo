namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    public class ResInfoCancel 
    {
        [XmlElement("resitem")]
        public string ResItem { get; set; } = string.Empty;
    }
}
