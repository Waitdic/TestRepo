namespace ThirdParty.CSSuppliers.Jumbo.Models
{
    using System.Xml.Serialization;

    public class Body
    {
        [XmlElement("availableHotelsByMultiQueryV12Response")]
        public AvailableHotelsByMultiQueryV12Response Content { get; set; }

    }
}