using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "CancelInfo")]
    public class CancelInfo
    {

        [XmlElement(ElementName = "BookingCodeState")]
        public string BookingCodeState { get; set; }

        [XmlElement(ElementName = "BookingCancelCost")]
        public decimal BookingCancelCost { get; set; }

        [XmlElement(ElementName = "BookingCancelCostCurrency")]
        public string BookingCancelCostCurrency { get; set; }
    }
}
