namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class Service
    {
        public string UID { get; set; } = string.Empty;

        [XmlElement("QUANTITY")]
        public decimal Quantity { get; set; }
    }
}
