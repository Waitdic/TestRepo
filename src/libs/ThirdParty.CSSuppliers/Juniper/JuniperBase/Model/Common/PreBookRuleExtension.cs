namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PreBookRuleExtension
    {
        [XmlElement("TotalPrice")]
        public Amount TotalPrice { get; set; } = new();

        [XmlArray("CancellationPolicyRules")]
        [XmlArrayItem("Rule")]
        public List<PolicyRule> CancellationPolicyRules { get; set; } = new();
    }
}
