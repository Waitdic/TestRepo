namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class CriterionExtension
    {
        [XmlElement("ShowCatalogueData")]
        public string ShowCatalogueData { get; set; }
        public bool ShouldSerializeShowCatalogueData() => string.Equals(ShowCatalogueData, "1");

        [XmlElement("ForceCurrency")]
        public string ForceCurrency { get; set; } = string.Empty;
        public bool ShouldSerializeForceCurrency() => !string.IsNullOrEmpty(ForceCurrency);

        [XmlElement("PaxCountry")]
        public string PaxCountry { get; set; } = string.Empty;
        public bool ShouldSerializePaxCountry() => !string.IsNullOrEmpty(PaxCountry);

        [XmlElement("ShowBasicInfo")]
        public string ShowBasicInfo { get; set; } = "0";
        [XmlElement("ShowPromotions")]
        public string ShowPromotions { get; set; } = "0";
        [XmlElement("ShowOnlyAvailable")]
        public string ShowOnlyAvailable { get; set; } = "1";
    }
}