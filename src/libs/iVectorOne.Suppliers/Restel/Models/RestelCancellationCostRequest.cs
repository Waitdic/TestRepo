namespace iVectorOne.CSSuppliers.Restel.Models
{
    using System.Xml.Serialization;

    [XmlRoot("peticion")]
    public class RestelCancellationCostRequest
    {
        [XmlElement("tipo")]
        public string Tipo { get; set; } = "142";

        [XmlElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [XmlElement("parametros")]
        public RequestParametros Parametros { get; set; } = new();

        public class RequestParametros
        {
            [XmlElement("localizador")]
            public string Localizador { get; set; } = string.Empty;

            [XmlElement("usuario")]
            public string Usuario { get; set; } = string.Empty;

            [XmlElement("idioma")]
            public int Idioma { get; set; }
        }
    }


}
