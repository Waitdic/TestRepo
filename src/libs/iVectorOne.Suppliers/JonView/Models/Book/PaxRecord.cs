namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    public class PaxRecord 
    {
        [XmlElement("paxnum")]
        public string PaxNum { get; set; } = string.Empty;

        [XmlElement("paxseq")]
        public string PaxSeq { get; set; } = string.Empty;

        [XmlElement("titlecode")]
        public string TitleCode { get; set; } = string.Empty;

        [XmlElement("fname")]
        public string FirstName { get; set; } = string.Empty;

        [XmlElement("lname")]
        public string LastName { get; set; } = string.Empty;

        [XmlElement("age")]
        public string Age { get; set; } = string.Empty;

        [XmlElement("language")]
        public string Language { get; set; } = string.Empty;
    }
}
