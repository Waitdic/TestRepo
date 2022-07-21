using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Holder")]
    public class Holder
    {
        public Holder(RelativePax relativePax)
        {
            RelativePax = relativePax;
        }

        public Holder()
        {
        }

        [XmlElement(ElementName = "RelPax")]
        public RelativePax RelativePax { get; set; }
    }
}
