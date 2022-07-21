using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "AdvancedOptions")]
    public class AvailabilityAdvancedOptions
    {
        public AvailabilityAdvancedOptions(bool showHotelInfo, bool showOnlyBestPriceCombination, 
            bool showCancellationPolicies, int timeout)
        {
            ShowHotelInfo = showHotelInfo;
            ShowOnlyBestPriceCombination = showOnlyBestPriceCombination;
            ShowCancellationPolicies = showCancellationPolicies;
            TimeOut = timeout;
        }

        public AvailabilityAdvancedOptions()
        {
        }

        [XmlElement(ElementName = "ShowHotelInfo", IsNullable = false)]
        public bool ShowHotelInfo { get; set; }
        [XmlElement(ElementName = "ShowOnlyBestPriceCombination", IsNullable = false)]
        public bool ShowOnlyBestPriceCombination { get; set; }
        [XmlElement(ElementName = "ShowCancellationPolicies", IsNullable = false)]
        public bool ShowCancellationPolicies { get; set; }
        [XmlElement(ElementName = "TimeOut", IsNullable = false)]
        public int TimeOut { get; set; }       
    }
}