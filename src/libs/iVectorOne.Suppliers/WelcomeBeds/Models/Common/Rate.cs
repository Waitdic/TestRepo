namespace iVectorOne.CSSuppliers.Models.WelcomeBeds
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Rate
    {
        public Rate() { }

        [XmlElement("Total")]
        public RateTotal Total { get; set; } = new RateTotal();

        [XmlArray("CancelPolicies")]
        [XmlArrayItem("CancelPenalty")]
        public List<CancelPenalty> CancelPolicies = new();

        [XmlElement("TPA_Extensions")]
        public TpaExtensions TpaExtensions { get; set; } = new TpaExtensions();
    }

}
