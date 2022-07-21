namespace iVectorOne.Suppliers.Restel.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("respuesta")]
    public class RestelCancellationCostResponse
    {
        [XmlElement("parametros")]
        public ResponseParametros Parametros { get; set; } = new();

        public class ResponseParametros
        {
            [XmlElement("politicaCanc")]
            public PoliticaCanc[] PoliticaCanc { get; set; } = Array.Empty<PoliticaCanc>();
        }
    }
}
