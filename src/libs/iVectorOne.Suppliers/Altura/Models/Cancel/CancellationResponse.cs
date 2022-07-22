namespace iVectorOne.CSSuppliers.Models.Altura
{
    using System.Xml.Serialization;

    public class CancellationResponse
    {
        public CancellationResponse() { }

        [XmlElement("Result")]
        public CancellationResult Result { get; set; } = new();

        [XmlElement("Session")]
        public Session Session { get; set; } = new();
    }
}
