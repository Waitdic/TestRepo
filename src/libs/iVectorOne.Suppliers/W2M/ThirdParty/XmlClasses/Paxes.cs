using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Paxes")]
    public class Paxes
    {
        public Paxes(List<Pax> paxList)
        {
            PaxList = paxList;
        }

        public Paxes()
        {
        }

        [XmlElement(ElementName = "Pax")]
        public List<Pax> PaxList { get; set; }
    }
}
