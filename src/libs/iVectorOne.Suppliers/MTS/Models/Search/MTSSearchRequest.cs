namespace iVectorOne.Suppliers.MTS.Models.Search
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [Serializable()]
    [XmlRoot("OTA_HotelAvailRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class MTSSearchRequest
    {
        [XmlAttribute]
        public string Version { get; set; } = string.Empty;

        public POS POS { get; set; } = new();

        [XmlArray("AvailRequestSegments")]
        [XmlArrayItem("AvailRequestSegment")]
        public AvailRequestSegment[] AvailRequestSegments { get; set; } = Array.Empty<AvailRequestSegment>();
    }

    public class AvailRequestSegment
    {
        [XmlAttribute]
        public string InfoSource { get; set; } = string.Empty;

        public StayDateRange StayDateRange { get; set; } = new();

        [XmlArray("RoomStayCandidates")]
        [XmlArrayItem("RoomStayCandidate")]
        public RoomStayCandidate[] RoomStayCandidates { get; set; } = Array.Empty<RoomStayCandidate>();

        public HotelSearchCriteria HotelSearchCriteria { get; set; } = new();
    }

    public class StayDateRange
    {
        [XmlAttribute]
        public string End { get; set; } = string.Empty;

        [XmlAttribute]
        public string Start { get; set; } = string.Empty;
    }

    public class RoomStayCandidate
    {
        [XmlAttribute]
        public int RPH { get; set; }

        [XmlArray("GuestCounts")]
        [XmlArrayItem("GuestCount")]
        public GuestCount[] GuestCounts { get; set; } = Array.Empty<GuestCount>();
    }

    public class HotelSearchCriteria
    {
        public Criterion? Criterion { get; set; }
        public bool ShouldSerializeCriterion() => Criterion != null;
    }

    public class Criterion
    {
        [XmlAttribute]
        public bool ExactMatch { get; set; }
        public bool ShouldSerializeExactMatch() => ExactMatch != false;

        public HotelRef? HotelRef { get; set; }
        public bool ShouldSerializeHotelRef() => HotelRef != null;

        [XmlElement("RefPoint")]
        public RefPoint[] RefPoint { get; set; } = Array.Empty<RefPoint>();
        public bool ShouldSerializeRefPoint() => RefPoint.Length != 0;
    }

    public class HotelRef
    {
        [XmlAttribute]
        public string HotelCode { get; set; } = string.Empty;
    }

    public class RefPoint
    {
        [XmlAttribute]
        public string CodeContext { get; set; } = string.Empty;

        [XmlText]
        public string RefPointValue { get; set; } = string.Empty;
    }
}
