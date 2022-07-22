namespace iVectorOne.CSSuppliers.Restel.Models
{
    using System.Xml.Serialization;

    [XmlRoot("peticion")]
    public class RestelBookRequest
    {
        [XmlElement("tipo")]
        public string Tipo { get; set; } = "3";

        [XmlElement("parametros")]
        public RequestParametros Parametros { get; set; } = new();

        public class RequestParametros
        {
            [XmlElement("localizador")]
            public string Localizador { get; set; } = string.Empty;

            [XmlElement("accion")]
            public string Accion { get; set; } = string.Empty;

            [XmlElement("afiliacion")]
            public string Afiliacion { get; set; } = string.Empty;

            [XmlElement("idioma")]
            public int Idioma { get; set; }
        }
    }
}
