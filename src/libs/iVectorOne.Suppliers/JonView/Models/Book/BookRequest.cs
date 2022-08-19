namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("message")]
    public class BookRequest : IMessageRq
    {
        [XmlElement("actionseg")]
        public string ActionSeg { get; set; } = string.Empty;

        [XmlElement("commitlevelseg")]
        public string CommitLevel { get; set; } = string.Empty;

        [XmlElement("resinfoseg")]
        public ResInfoSegment ResInfo { get; set; } = new();

        [XmlArray("paxseg")]
        [XmlArrayItem("paxrecord")]
        public List<PaxRecord> PaxSegment { get; set; } = new();

        [XmlArray("bookseg")]
        [XmlArrayItem("bookrecord")]
        public List<BookRecord> BookSegment { get; set; } = new();
    }
}
