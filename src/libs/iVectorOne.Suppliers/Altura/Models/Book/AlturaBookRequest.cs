namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    [XmlRoot("AlturaDS_requests")]
    public class AlturaBookRequest
    {
        public AlturaBookRequest() { }

        [XmlElement("Request")]
        public BookRequest Request { get; set; } = new();

    }
}
