namespace iVectorOne.CSSuppliers.Restel.Models.Common
{
    using System.Xml.Serialization;

    public class Valorafegit
    {
        [XmlElement("nom")]
        public string Nom { get; set; } = string.Empty;

        [XmlElement("descripcio")]
        public string Descripcio { get; set; } = string.Empty;

        [XmlElement("duracio")]
        public string Duracio { get; set; } = string.Empty;
    }
}
