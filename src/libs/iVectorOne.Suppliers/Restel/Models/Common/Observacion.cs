namespace iVectorOne.Suppliers.Restel.Models.Common
{
    using System.Xml.Serialization;

    public class Observacion
    {
        [XmlElement("obs_texto")]
        public string ObsTexto { get; set; } = string.Empty;
    }
}