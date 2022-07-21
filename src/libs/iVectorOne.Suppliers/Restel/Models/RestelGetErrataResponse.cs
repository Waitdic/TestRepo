namespace iVectorOne.CSSuppliers.Restel.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("respuesta")]
    public class RestelGetErrataResponse
    {
        [XmlElement("parametros")]
        public ErrataResponseParametros Parametros { get; set; } = new();

        public class ErrataResponseParametros
        {
            [XmlElement("hotel")]
            public Hotel Hotel { get; set; } = new();
        }
    }
}
