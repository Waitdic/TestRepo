namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class RatePlanCandidate
    {
        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;
    }
}
