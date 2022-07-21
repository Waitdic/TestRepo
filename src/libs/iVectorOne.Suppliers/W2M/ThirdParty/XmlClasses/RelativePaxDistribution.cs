using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "RelPaxDist")]
    public class RelativePaxDistribution
    {
        public RelativePaxDistribution(RelativePaxes relativePaxes)
        {
            RelativePaxes = relativePaxes;
        }

        public RelativePaxDistribution()
        {
        }

        [XmlElement(ElementName = "RelPaxes")] 
        public RelativePaxes RelativePaxes { get; set; }
    }
}
