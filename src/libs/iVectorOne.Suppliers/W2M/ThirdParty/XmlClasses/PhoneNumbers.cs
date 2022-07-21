using System.Xml.Serialization;

namespace iVectorOne.Suppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "PhoneNumbers")]
    public class PhoneNumbers
    {
        [XmlElement(ElementName = "PhoneNumber")]
        public PhoneNumber PhoneNumber { get; set; } = new PhoneNumber();
    }
}
