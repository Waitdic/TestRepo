using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "RelPaxesDist")]
    public class RelativePaxesDistribution
    {
        public RelativePaxesDistribution(List<RelativePaxDistribution> relativePaxDistributionList)
        {
            RelativePaxDistributionList = relativePaxDistributionList;
        }

        public RelativePaxesDistribution()
        {
        }

        [XmlElement(ElementName = "RelPaxDist")]
        public List<RelativePaxDistribution> RelativePaxDistributionList { get; set; }
    }
}
