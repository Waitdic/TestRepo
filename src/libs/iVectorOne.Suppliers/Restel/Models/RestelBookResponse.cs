namespace iVectorOne.CSSuppliers.Restel.Models
{
    using System.Xml.Serialization;

    [XmlRoot("respuesta")]
    public class RestelBookResponse
    {
        [XmlElement("parametros")]
        public ResponseParametros Parametros { get; set; } = new();

        public class ResponseParametros
        {
            [XmlElement("estado")]
            public string Estado { get; set; } = string.Empty;

            [XmlElement("localizador_corto")]
            public string LocalizadorCorto { get; set; } = string.Empty;

            [XmlElement("localizador")]
            public string Localizador { get; set; } = string.Empty;
        }
    }
}
