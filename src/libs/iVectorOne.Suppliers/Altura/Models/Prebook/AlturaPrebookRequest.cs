namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    [XmlRoot("AlturaDS_requests")]
    public class AlturaPrebookRequest
    {
        public AlturaPrebookRequest() { }

        [XmlElement("Request")]
        public PrebookRequest Request { get; set; } = new();
    }
}
