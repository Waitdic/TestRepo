namespace iVectorOne.CSSuppliers.Restel.Models
{
    using System.Xml.Serialization;

    [XmlRoot("peticion")]
    public class RestelGetErrataRequest
    {
        [XmlElement("tipo")]
        public string Tipo { get; set; } = "24";

        [XmlElement("parametros")]
        public ErrataRequestParametros Parametros { get; set; } = new();

        public class ErrataRequestParametros
        {
            [XmlElement("codigo_cobol")]
            public string CodigoCobol { get; set; } = string.Empty;

            [XmlElement("entrada")]
            public string Entrada { get; set; } = string.Empty;

            [XmlElement("salida")]
            public string Salida { get; set; } = string.Empty;
        }
    }
}

