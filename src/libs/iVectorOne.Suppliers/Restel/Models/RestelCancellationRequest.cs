namespace iVectorOne.Suppliers.Restel.Models
{
    using System.Xml.Serialization;

    [XmlRoot("peticion")]
    public class RestelCancellationRequest
    {
        [XmlElement("tipo")]
        public string Tipo { get; set; } = "401";

        [XmlElement("parametros")]
        public RequestParametros Parametros { get; set; } = new();

        public class RequestParametros
        {
            [XmlElement("localizador_largo")]
            public string LocalizadorLargo { get; set; } = string.Empty;

            [XmlElement("localizador_corto")]
            public string LocalizadorCorto { get; set; } = string.Empty;
        }
    }
}
