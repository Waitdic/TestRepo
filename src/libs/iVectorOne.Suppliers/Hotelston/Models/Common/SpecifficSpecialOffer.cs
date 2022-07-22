namespace iVectorOne.CSSuppliers.Hotelston.Models.Common
{
    using System.Xml.Serialization;

    public class SpecifficSpecialOffer
    {
        [XmlAttribute("details")]
        public string Details { get; set; } = string.Empty;
    }
}