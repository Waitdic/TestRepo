namespace iVectorOne.Suppliers.Restel.Models
{
    using System.Xml.Serialization;

    [XmlRoot("peticion")]
    public class RestelGetAddedValuesRequest
    {
        [XmlElement("tipo")]
        public string Tipo { get; set; } = "28";

        [XmlElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [XmlElement("parametros")]
        public RequestParametros Parametros { get; set; } = new();

        public class RequestParametros
        {
            [XmlElement("idioma")]
            public int Idioma { get; set; }

            [XmlElement("fentrada")]
            public string Fentrada { get; set; } = string.Empty;

            [XmlElement("codhot")]
            public string Codhot { get; set; } = string.Empty;
        }
    }
}
