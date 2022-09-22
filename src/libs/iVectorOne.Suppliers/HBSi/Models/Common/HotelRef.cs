namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class HotelRef
    {
        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;
    }
}
