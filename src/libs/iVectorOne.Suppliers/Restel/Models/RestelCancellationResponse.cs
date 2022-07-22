namespace iVectorOne.CSSuppliers.Restel.Models
{
    using System.Xml.Serialization;

    [XmlRoot("respuesta")]
    public class RestelCancellationResponse
    {
        [XmlElement("parametros")]
        public RequestParametros Parametros { get; set; } = new();

        public class RequestParametros
        {
            [XmlElement("estado")]
            public string Estado { get; set; } = string.Empty;

            [XmlElement("localizador")]
            public string Localizador { get; set; } = string.Empty;

            [XmlElement("localizador_baja")]
            public string LocalizadorBaja { get; set; } = string.Empty;
        }
    }
}
