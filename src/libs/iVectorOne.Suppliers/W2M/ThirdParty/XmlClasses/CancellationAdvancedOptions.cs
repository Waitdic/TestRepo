using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "AdvancedOptions")]
    public class CancellationAdvancedOptions
    {
        public CancellationAdvancedOptions()
        {
        }

        [XmlElement(ElementName = "ShowBreakdownPrice", IsNullable = false)]
        public bool ShowBreakdownPrice { get; set; }
        [XmlElement(ElementName = "ShowCompleteInfo", IsNullable = false)]
        public bool ShowCompleteInfo { get; set; }
        [XmlElement(ElementName = "ShowOnlyBasicInfo", IsNullable = false)]
        public bool ShowOnlyBasicInfo { get; set; }
        [XmlElement(ElementName = "ShowHotelInfo", IsNullable = false)]
        public bool ShowHotelInfo { get; set; }
        [XmlElement(ElementName = "ShowOnlyBestPriceCombination", IsNullable = false)]
        public bool ShowOnlyBestPriceCombination { get; set; }
        [XmlElement(ElementName = "TimeOut", IsNullable = false)]
        public int TimeOut { get; set; }
        [XmlElement(ElementName = "OnlyCancellationFees", IsNullable = false)]
        public bool OnlyCancellationFees { get; set; }
    }
}
