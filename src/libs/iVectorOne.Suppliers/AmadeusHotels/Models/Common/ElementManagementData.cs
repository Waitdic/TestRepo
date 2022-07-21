namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class ElementManagementData
    {
        [XmlElement("reference")]
        public Reference Reference { get; set; } = new();

        [XmlElement("segmentName")]
        public string SegmentName { get; set; } = string.Empty;
    }
}
