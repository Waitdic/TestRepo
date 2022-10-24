namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Accommodation
    {
        public string UID { get; set; } = string.Empty;

        [XmlArray("ROOMS")]
        [XmlArrayItem("ROOM")]
        public SearchRoom[] Rooms { get; set; } = Array.Empty<SearchRoom>();

        [XmlArray("CANCELLATIONPOLICIES")]
        [XmlArrayItem("CANCELLATIONPOLICY")]
        public CancellationPolicy[] CancellationPolicies{ get; set; } = Array.Empty<CancellationPolicy>();
    }
}
