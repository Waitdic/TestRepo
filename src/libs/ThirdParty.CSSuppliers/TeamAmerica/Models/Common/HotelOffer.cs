namespace ThirdParty.CSSuppliers.TeamAmerica.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class HotelOffer
    {
        [XmlElement("TeamVendorID")]
        public string TeamVendorID { get; set; } = string.Empty;

        [XmlElement("ProductCode")]
        public string ProductCode { get; set; } = string.Empty;

        [XmlElement("FamilyPlan")]
        public string FamilyPlan { get; set; } = string.Empty;

        [XmlElement("ChildAge")]
        public int ChildAge { get; set; }

        [XmlElement("NightlyInfo")]
        public List<NightlyInfo> NightlyInfos { get; set; } = new();

        [XmlElement("RoomType")]
        public string RoomType { get; set; } = string.Empty;

        [XmlElement("MealPlan")]
        public string MealPlan { get; set; } = string.Empty;

        [XmlElement("MaxOccupancy")]
        public int MaxOccupancy { get; set; }

        [XmlElement("NonRefundable")]
        public string NonRefundable { get; set; } = string.Empty;

        [XmlArray("CancellationPolicies")]
        [XmlArrayItem("CancellationPolicy")]
        public List<CancellationPolicy> CancellationPolicies { get; set; } = new();

        [XmlElement("AverageRate")]
        public List<AverageRate> AverageRates { get; set; } = new();
    }
}
