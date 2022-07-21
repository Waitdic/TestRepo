namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RatePlanCandidates
    {
        [XmlArray("RatePlanCandidate")]
        [XmlArrayItem("RatePlanCandidate")]
        public RatePlanCandidate[] RatePlanCandidate { get; set; } = Array.Empty<RatePlanCandidate>();
    }
}
