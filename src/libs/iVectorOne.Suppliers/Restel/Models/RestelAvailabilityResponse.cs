namespace iVectorOne.CSSuppliers.Restel.Models
{
    using System.Xml.Serialization;
    using iVectorOne.CSSuppliers.Restel.Models.Common;

    [XmlRoot("respuesta")]
    public class RestelAvailabilityResponse
    {
        [XmlElement("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [XmlElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [XmlElement("agencia")]
        public string Agencia { get; set; } = string.Empty;

        [XmlElement("param")]
        public Param Param { get; set; } = new();
    }
}
