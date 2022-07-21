namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    [XmlRoot("AlturaDS_responses")]
    public class AlturaBookResponse
    {
        public AlturaBookResponse() { }

        [XmlElement("Response")]
        public BookResponse Response { get; set; } = new();
    }
}
