using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "RelPax")]
    public class RelativePax
    {
        public RelativePax(int paxId)
        {
            PaxId = paxId;
        }

        public RelativePax()
        {
        }

        [XmlAttribute(AttributeName = "IdPax")]
        public int PaxId { get; set; }
    }
}
