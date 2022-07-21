namespace iVectorOne.Suppliers.Hotelston.Models
{
    using System.Xml.Serialization;
    using Common;

    public class CheckAvailabilityRequest : RequestBase
    {
        [XmlElement("criteria")]
        public Criteria Criteria { get; set; } = new();
    }
}
