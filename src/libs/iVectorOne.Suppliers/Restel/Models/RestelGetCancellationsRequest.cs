namespace iVectorOne.Suppliers.Restel.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("peticion")]
    public class RestelGetCancellationsRequest
    {
        [XmlElement("tipo")]
        public string Tipo { get; set; } = "144";

        [XmlElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [XmlElement("parametros")]
        public RequestParametros Parametros { get; set; } = new();

        public class RequestParametros
        {
            [XmlElement("datos_reserva")]
            public DatosReserva DatosReserva { get; set; } = new();
        }
    }
}
