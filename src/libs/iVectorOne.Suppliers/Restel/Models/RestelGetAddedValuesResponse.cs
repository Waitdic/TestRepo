namespace iVectorOne.Suppliers.Restel.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("respuesta")]
    public class RestelGetAddedValuesResponse
    {
        [XmlElement("parametros")]
        public ResponseParametros Parametros { get; set; } = new();

        public class ResponseParametros
        {
            [XmlElement("valorafegit")]
            public Valorafegit[] Valorafegit { get; set; } = Array.Empty<Valorafegit>();
        }
    }
}
