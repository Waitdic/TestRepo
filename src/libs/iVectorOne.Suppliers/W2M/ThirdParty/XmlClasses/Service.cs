using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Service")]
    public class Service
    {
        public Service(decimal amount)
        {
            Amount = amount;
        }

        public Service()
        {
        }

        [XmlAttribute(AttributeName = "Amount")]
        public decimal Amount { get; set; }
    }
}
