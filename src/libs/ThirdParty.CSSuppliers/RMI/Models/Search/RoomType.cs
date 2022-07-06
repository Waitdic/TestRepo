namespace ThirdParty.CSSuppliers.RMI.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomType
    {
        public string OnRequest { get; set; }

        [XmlElement("RoomID")]
        public string RoomId { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("RoomsAppliesTo")]
        public RoomsAppliesTo RoomsAppliesTo { get; set; } = new();

        [XmlElement("Total")]
        public decimal Total { get; set; }

        [XmlElement("MealBasisID")]
        public string MealBasisId { get; set; }

        [XmlArray("SpecialOffers")]
        [XmlArrayItem("SpecialOffer")]
        public List<SpecialOffer> SpecialOffers { get; set; } = new();

        [XmlArray("CancellationPolicies")]
        [XmlArrayItem("CancellationPolicy")]
        public List<CancellationPolicy> CancellationPolicies { get; set; } = new();
    }
}
