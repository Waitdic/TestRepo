using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "FirstDayCostCancellation")]
    public class FirstDayCostCancellation
    {
        [XmlAttribute(AttributeName = "Hour")]
        public string Hour { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}
