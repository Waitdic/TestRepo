namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class Service
    {
        [XmlElement("OPTIONAL")]
        public bool Optional { get; set; }

        [XmlElement("AMOUNT")]
        public decimal Amount { get; set; }
    }
}
