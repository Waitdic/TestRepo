namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Rate
    {
        [XmlAttribute("RateTimeUnit")]
        public string RateTimeUnit { get; set; } = string.Empty;
        public bool ShouldSerializeRateTimeUnit() => !string.IsNullOrEmpty(RateTimeUnit);

        [XmlAttribute("UnitMultiplier")]
        public int UnitMultiplier { get; set; }

        [XmlAttribute("EffectiveDate")]
        public string EffectiveDate { get; set; } = string.Empty;
        public bool ShouldSerializeEffectiveDate() => !string.IsNullOrEmpty(EffectiveDate);

        [XmlAttribute("ExpireDate")]
        public string ExpireDate { get; set; } = string.Empty;
        public bool ShouldSerializeExpireDate() => !string.IsNullOrEmpty(ExpireDate);

        [XmlElement("Base")]
        public RateAmount RateBase { get; set; } = new();

        [XmlElement("Total")]
        public RateAmount Total { get; set; } = new();


        [XmlArray("Taxes")]
        [XmlArrayItem("Tax")]
        public List<Tax> Taxes { get; set; } = new();
    }

    public class Tax
    {
        [XmlAttribute("Type")]
        public string TaxType { get; set; } = string.Empty;

        [XmlAttribute("Amount")]
        public string Amount { get; set; } = string.Empty;
    }
}
