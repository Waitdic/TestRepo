namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class CriterionExtension
    {
        [XmlElement("ShowCatalogueData")]
        public string ShowCatalogueData { get; set; }
        public bool ShouldSerializeShowCatalogueData() => string.Equals(ShowCatalogueData, "1");

        [XmlElement("ForceCurrency")]
        public string ForceCurrency { get; set; } = string.Empty;
        [XmlElement("PaxCountry")]
        public string PaxCountry { get; set; } = string.Empty;
        [XmlElement("ShowBasicInfo")]
        public string ShowBasicInfo { get; set; } = "0";
        [XmlElement("ShowPromotions")]
        public string ShowPromotions { get; set; } = "0";
        [XmlElement("ShowOnlyAvailable")]
        public string ShowOnlyAvailable { get; set; } = "1";
    }
}