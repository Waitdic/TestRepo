namespace iVectorOne.Suppliers.MTS.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class HotelReservation
    {
        [XmlArray("RoomStays")]
        [XmlArrayItem("RoomStay")]
        public RoomStay[] RoomStays { get; set; } = Array.Empty<RoomStay>();

        [XmlArray("Services")]
        [XmlArrayItem("Service")]
        public Service[] Services { get; set; } = Array.Empty<Service>();

        [XmlArray("ResGuests")]
        [XmlArrayItem("ResGuest")]
        public ResGuest[] ResGuests { get; set; } = Array.Empty<ResGuest>();

        public ResGlobalInfo? ResGlobalInfo { get; set; }
        public bool ShouldSerializeResGlobalInfo() => ResGlobalInfo != null;
    }

    public class ResGlobalInfo
    {
        public Total? Total { get; set; }
        public bool ShouldSerializeTotal() => Total != null;

        public BasicPropertyInfo? BasicPropertyInfo { get; set; }
        public bool ShouldSerializeBasicPropertyInfo() => BasicPropertyInfo != null;

        [XmlArray("CancelPenalties")]
        [XmlArrayItem("CancelPenalty")]
        public CancelPenalty[] CancelPenalties { get; set; } = Array.Empty<CancelPenalty>();
        public bool ShouldSerializeCancelPenalties() => CancelPenalties.Length != 0;

        [XmlArray("HotelReservationIDs")]
        [XmlArrayItem("HotelReservationID")]
        public HotelReservationID[] HotelReservationIDs { get; set; } = Array.Empty<HotelReservationID>();
        public bool ShouldSerializeHotelReservationIDs() => HotelReservationIDs.Length != 0;

        [XmlArray("Comments")]
        [XmlArrayItem("Comment")]
        public Comment[] Comments { get; set; } = Array.Empty<Comment>();
        public bool ShouldSerializeComments() => Comments.Length != 0;
    }

    public class Total
    {
        public decimal AmountAfterTax { get; set; }
    }

    public class RoomStay
    {
        [XmlArray("RoomTypes")]
        [XmlArrayItem("RoomType")]
        public RoomType[] RoomTypes { get; set; } = Array.Empty<RoomType>();

        [XmlArray("RatePlans")]
        [XmlArrayItem("RatePlan")]
        public RatePlan[] RatePlans { get; set; } = Array.Empty<RatePlan>();
        public bool ShouldSerializeRatePlans() => RatePlans.Length != 0;

        [XmlElement]
        public TimeSpanLocal TimeSpan { get; set; } = new();

        public BasicPropertyInfo BasicPropertyInfo { get; set; } = new();

        [XmlArray("ResGuestRPHs")]
        [XmlArrayItem("ResGuestRPH")]
        public ResGuestRPH[] ResGuestRPHs { get; set; } = Array.Empty<ResGuestRPH>();

        [XmlArray("ServiceRPHs")]
        [XmlArrayItem("ServiceRPH")]
        public ServiceRPH[] ServiceRPHs { get; set; } = Array.Empty<ServiceRPH>();
    }

    public class TimeSpanLocal
    {
        [XmlAttribute]
        public string End { get; set; } = string.Empty;

        [XmlAttribute]
        public string Start { get; set; } = string.Empty;
    }

    public class ResGuestRPH
    {
        [XmlAttribute]
        public int RPH { get; set; }
    }

    public class ServiceRPH
    {
        [XmlAttribute]
        public int RPH { get; set; }
    }

    public class Service
    {
        [XmlAttribute]
        public string ServiceInventoryCode { get; set; } = string.Empty;

        [XmlAttribute]
        public int ServiceRPH { get; set; }
    }

    public class ResGuest
    {
        [XmlAttribute]
        public int AgeQualifyingCode { get; set; }

        [XmlAttribute]
        public int ResGuestRPH { get; set; }

        [XmlArray("GuestCounts")]
        [XmlArrayItem("GuestCount")]
        public GuestCount[] GuestCounts { get; set; } = Array.Empty<GuestCount>();
    }

    public class CancelPenalty
    {
        public AmountPercent? AmountPercent { get; set; }

        public Deadline Deadline { get; set; } = new();
    }

    public class AmountPercent
    {
        [XmlAttribute]
        public int Percent { get; set; }

        [XmlAttribute]
        public int NmbrOfNights { get; set; }

        [XmlAttribute]
        public decimal? Amount { get; set; }
    }

    public class Deadline
    {
        [XmlAttribute]
        public string OffsetDropTime { get; set; }

        [XmlAttribute]
        public string OffsetTimeUnit { get; set; } = string.Empty;

        [XmlAttribute]
        public int OffsetUnitMultiplier { get; set; }

        [XmlAttribute]
        public int OffsetMultiplier { get; set; }
    }

    public class HotelReservationID
    {
        [XmlAttribute("ResID_SourceContext")]
        public string ResIDSourceContext { get; set; } = string.Empty;

        [XmlAttribute("ResID_Source")]
        public string ResIDSource { get; set; } = string.Empty;

        [XmlAttribute("ResID_Value")]
        public string ResIDValue { get; set; } = string.Empty;
    }

    public class Comment
    {
        [XmlAttribute]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }
}
