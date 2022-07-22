using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "AgenciesData")]
    public class AgenciesData
    {
        [XmlElement(ElementName = "AgencyData")]
        public List<AgencyData> AgencyData { get; set; }
    }
}
