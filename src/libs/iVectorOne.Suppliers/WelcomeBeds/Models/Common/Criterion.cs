namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class Criterion
    {
        public Criterion() { }
        [XmlElement(ElementName = "HotelRef")]
        public HotelRef HotelRef { get; set; } = new HotelRef();
    }
}
