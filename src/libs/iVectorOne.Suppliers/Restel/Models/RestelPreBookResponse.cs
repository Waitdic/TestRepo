namespace iVectorOne.Suppliers.Restel.Models
{
    using System.Xml.Serialization;

    [XmlRoot("respuesta")]
    public class RestelPreBookResponse
    {
        [XmlElement("parametros")]
        public ResponseParametros Parametros { get; set; } = new();

        public class ResponseParametros
        {
            [XmlElement("estado")]
            public string Estado { get; set; } = string.Empty;

            [XmlElement("n_localizador")]
            public string Localizador { get; set; } = string.Empty;

            [XmlElement("importe_total_reserva")]
            public string ImporteTotalReserva { get; set; } = string.Empty;
        }
    }
}
