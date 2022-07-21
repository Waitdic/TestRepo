namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class ElementManagementPassenger
    {
        [XmlElement("reference")]
        public Reference Reference { get; set; } = new();

        [XmlElement("segmentName")]
        public string SegmentName { get; set; } = string.Empty;
    }
}
