namespace iVectorOne.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class HotelRef
    {
        public HotelRef() { }
        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;
    }
}
