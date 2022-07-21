namespace ThirdParty.CSSuppliers.Models.Altura
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    public class PrebookResponse
    {
        public PrebookResponse() { }

        [XmlElement("Result")]
        public PrebookResult Result { get; set; } = new();

        [XmlElement("RateDetails")]
        public RateDetails RateDetails { get; set; } = new();

        [XmlArray("CancellationPrices")]
        [XmlArrayItem("Price")]
        public List<Price> CancellationPrices { get; set; } = new();

        [XmlArray("ContractRemarks")]
        [XmlArrayItem("Remark")]
        public List<string> ContractRemarks { get; set; } = new();
    }
}
