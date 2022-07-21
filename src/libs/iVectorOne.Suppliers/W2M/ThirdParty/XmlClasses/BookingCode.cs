using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "BookingCode")]
    public class BookingCode
    {
        [XmlAttribute(AttributeName = "ExpirationDate")]
        public string ExpirationDate { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}
