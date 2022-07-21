using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "SearchSegmentHotels")]
    public class SearchSegmentHotels
    {
        public SearchSegmentHotels(string start, string end)
        {
            Start = start;
            End = end;
        }

        public SearchSegmentHotels()
        {
        }

        [XmlAttribute(AttributeName = "Start")]
        public string Start { get; set; }
        [XmlAttribute(AttributeName = "End")]
        public string End { get; set; }
    }
}
