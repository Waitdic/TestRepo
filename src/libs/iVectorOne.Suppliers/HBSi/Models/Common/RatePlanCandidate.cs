namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RatePlanCandidate
    {
        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;

        [XmlAttribute("RPH")]
        public string RPH { get; set; } = string.Empty;

        [XmlArray("HotelRefs")]
        [XmlArrayItem("HotelRef")]
        public List<HotelRef> HotelRefs { get; set; } = new();

        [XmlElement("MealsIncluded")]
        public MealsIncluded MealsIncluded { get; set; } = new();
    }
}
