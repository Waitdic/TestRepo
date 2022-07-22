namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class FreetextDetail
    {
        [XmlElement("subjectQualifier")]
        public string SubjectQualifier { get; set; } = string.Empty;
        
        [XmlElement("type")]
        public string Type { get; set; } = string.Empty;
    }
}
