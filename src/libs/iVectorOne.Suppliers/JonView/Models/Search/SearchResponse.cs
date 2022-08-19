namespace iVectorOne.Suppliers.JonView.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("message")]
    public class SearchResponse : IMessageRs
    {
        [XmlElement("alternatelistseg")]
        public AlternateList AlternateList { get; set; }
    }

    public class AlternateList
    {
        [XmlElement("listrecord")]
        public List<RoomRecord> RoomRecords { get; set; } = new List<RoomRecord>();
    }
}