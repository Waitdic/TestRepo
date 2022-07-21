namespace iVectorOne.CSSuppliers.Restel.Models
{
    using System.Xml.Serialization;
    using iVectorOne.CSSuppliers.Restel.Models.Common;

    [XmlRoot("peticion")]
    public class RestelPrebookRequest
    {
        [XmlElement("tipo")]
        public string Tipo { get; set; } = "202";

        [XmlElement("parametros")]
        public RequestParametros Parametros { get; set; } = new();

        public class RequestParametros
        {
            [XmlElement("res")]
            public Res Res { get; set; } = new();

            [XmlElement("codigo_hotel")]
            public string CodigoHotel { get; set; } = string.Empty;

            [XmlElement("nombre_cliente")]
            public string NombreCliente { get; set; } = string.Empty;

            [XmlElement("observaciones")]
            public string Observaciones { get; set; } = string.Empty;

            [XmlElement("num_mensaje")]
            public string NumMensaje { get; set; } = string.Empty;

            [XmlElement("forma_pago")]
            public int FormaPago { get; set; }

            [XmlElement("Idioma")]
            public int Idioma { get; set; }
        }
    }
}
