namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Rate
    {
        [XmlArray("Fees")]
        [XmlArrayItem("Fee")]
        public Fee[] Fees { get; set; } = Array.Empty<Fee>();

        public Base Base { get; set; } = new();

        public Discount Discount { get; set; } = new();
        
        [XmlAttribute("EffectiveDate")]
        public string EffectiveDate { get; set; } = string.Empty;
    }
}
