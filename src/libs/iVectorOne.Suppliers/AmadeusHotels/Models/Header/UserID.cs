namespace iVectorOne.Suppliers.AmadeusHotels.Models.Header
{
    using System.Xml.Serialization;

    public class UserID
    {
        [XmlAttribute("POS_Type")]
        public string PosType { get; set; } = string.Empty;

        [XmlAttribute("PseudoCityCode")]
        public string PseudoCityCode { get; set; } = string.Empty;

        [XmlAttribute("AgentSign")]
        public string AgentSign { get; set; } = string.Empty;

        [XmlAttribute("RequestorType")]
        public string RequestorType { get; set; } = string.Empty;
    }
}
