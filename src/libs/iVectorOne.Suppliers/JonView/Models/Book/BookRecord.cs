namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Xml.Serialization;

    public class BookRecord 
    {
        [XmlElement("booknum")]
        public string BookNum { get; set; } = string.Empty;

        [XmlElement("bookseq")]
        public string BookSeq { get; set; } = string.Empty;

        [XmlElement("prodcode")]
        public string ProdCode { get; set; } = string.Empty;

        [XmlElement("startdate")]
        public string StartDate { get; set; } = string.Empty;

        [XmlElement("duration")]
        public string Duration { get; set; } = string.Empty;

        [XmlElement("note")]
        public string Note { get; set; } = string.Empty;

        [XmlElement("paxarray")]
        public string PaxArray { get; set; } = string.Empty;
    }
}
