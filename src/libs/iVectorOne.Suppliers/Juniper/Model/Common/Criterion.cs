namespace iVectorOne.Suppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class Criterion
    {
        public Criterion() { }

        [XmlElement("HotelRef")]
        public HotelRef HotelRef { get; set; } = new();

        [XmlElement("TPA_Extensions")]
        public CriterionExtension Extensions { get; set; } = new();
    }
}