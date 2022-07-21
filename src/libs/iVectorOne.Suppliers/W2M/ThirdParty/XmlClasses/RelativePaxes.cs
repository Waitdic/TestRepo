using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "RelPaxes")]
    public class RelativePaxes
    {
        public RelativePaxes(List<RelativePax> relativePaxList)
        {
            RelativePaxList = relativePaxList;
        }

        public RelativePaxes()
        {
        }

        [XmlElement(ElementName = "RelPax")]
        public List<RelativePax> RelativePaxList { get; set; }
    }
}
