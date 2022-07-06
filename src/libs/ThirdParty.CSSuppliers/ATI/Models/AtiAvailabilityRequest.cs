namespace ThirdParty.CSSuppliers.ATI.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlType(Namespace = SoapNamespaces.Ns)]
    public class AtiAvailabilityRequest : SoapContent
    {
        [XmlAttribute("Version")]
        public string Version { get; set; } = string.Empty;

        [XmlElement("POS")]
        public Pos Pos { get; set; } = new();

        [XmlArray("AvailRequestSegments")]
        [XmlArrayItem("AvailRequestSegment")]
        public AvailRequestSegment[] AvailRequestSegments { get; set; } = Array.Empty<AvailRequestSegment>();
    }
}
