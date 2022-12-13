namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RatePlan
    {
        [XmlAttribute]
        public string RatePlanCode { get; set; } = string.Empty;

        public StatusWarningFlags? StatusWarningFlags { get; set; }
        public bool ShouldSerializeStatusWarningFlags() => StatusWarningFlags != null;

        [XmlElement("CancellationPolicy")]
        public CancellationPolicy[] CancellationPolicy { get; set; } = Array.Empty<CancellationPolicy>();
        public bool ShouldSerializeCancellationPolicy() => CancellationPolicy != Array.Empty<CancellationPolicy>();

        public Rooms? Rooms { get; set; }
        public bool ShouldSerializeRooms() => Rooms != null;
    }
}
