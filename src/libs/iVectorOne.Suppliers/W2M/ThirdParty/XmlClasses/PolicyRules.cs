using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "PolicyRules")]
    public class PolicyRules
    {
        [XmlElement(ElementName = "Rule")]
        public List<PolicyRule> RuleList { get; set; }
    }
}
