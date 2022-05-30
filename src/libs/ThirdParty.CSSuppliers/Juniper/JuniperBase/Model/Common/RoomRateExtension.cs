using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    public class RoomRateExtension
    {
        public RoomRateExtension() { }

        [XmlElement("Commission")]
        public Amount Commission { get; set; } = new();

        [XmlElement("NettPrice")]
        public Amount NettPrice { get; set; } = new();

        [XmlElement("MealPlan")]
        public MealPlan MealPlan { get; set; } = new();

        [XmlArray("Elements")]
        [XmlArrayItem("Element")]
        public List<RateElement> Elements { get; set; } = new();

        [XmlElement("NonRefundable")]
        public string NonRefundable { get; set; } = string.Empty;
    }
}
