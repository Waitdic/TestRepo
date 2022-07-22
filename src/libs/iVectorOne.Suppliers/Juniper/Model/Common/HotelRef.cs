namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class HotelRef
    {
        public HotelRef() { }

        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;
    }
}