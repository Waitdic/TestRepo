namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class RoomRate
    {
        [XmlAttribute("RoomTypeCode")]
        public string RoomTypeCode { get; set; } = string.Empty;

        [XmlAttribute("NumberOfUnits")]
        public int NumberOfUnits { get; set; }

        [XmlAttribute("EffectiveDate")]
        public string EffectiveDate { get; set; } = string.Empty;
        public bool ShouldSerializeEffectiveDate() => !string.IsNullOrEmpty(EffectiveDate);

        [XmlAttribute("ExpireDate")]
        public string ExpireDate { get; set; } = string.Empty;
        public bool ShouldSerializeExpireDate() => !string.IsNullOrEmpty(ExpireDate);

        [XmlAttribute("RatePlanCode")]
        public string RatePlanCode { get; set; } = string.Empty;

        [XmlArray("Rates")]
        [XmlArrayItem("Rate")]
        public List<Rate> Rates { get; set; } = new();

        [XmlElement("RoomRateDescription")]
        public RoomRateDescription RoomRateDescription { get; set; } = new();
    }
}
