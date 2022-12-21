namespace iVectorOne.Suppliers.PremierInn.Models.Common
{
    using System.Xml.Serialization;

    public class RatePlan
    {
        [XmlAttribute]
        public string RatePlanCode { get; set; } = string.Empty;

        [XmlAttribute]
        public string CellCode { get; set; } = string.Empty;

        public StatusWarningFlags? StatusWarningFlags { get; set; }
        public bool ShouldSerializeStatusWarningFlags() => StatusWarningFlags != null;

        [XmlElement("CancellationPolicy")]
        public CancellationPolicy? CancellationPolicy { get; set; }
        public bool ShouldSerializeCancellationPolicy() => CancellationPolicy != null;

        public Rooms? Rooms { get; set; }
        public bool ShouldSerializeRooms() => Rooms != null;
    }
}
