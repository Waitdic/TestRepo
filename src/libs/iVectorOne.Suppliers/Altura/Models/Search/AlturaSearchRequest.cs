namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Xml.Serialization;

    [XmlRoot("AlturaDS_requests")]
    public class AlturaSearchRequest
    {
        public AlturaSearchRequest() { }

        [XmlElement("Request")]
        public SearchRequest Request { get; set; } = new();
    }
}
