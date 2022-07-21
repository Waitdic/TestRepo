namespace iVectorOne.CSSuppliers.Restel.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class DatosReserva
    {
        [XmlElement("hotel")]
        public string Hotel { get; set; } = string.Empty;

        [XmlElement("lin")]
        public List<string> Lin { get; set; } = new();
    }
}
