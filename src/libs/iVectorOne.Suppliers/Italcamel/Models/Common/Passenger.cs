namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System.Xml.Serialization;

    public class Passenger
    {
        [XmlElement("NAME")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("SURNAME")]
        public string Surname { get; set; } = string.Empty;

        [XmlElement("BIRTHDATE")]
        public string Birthdate { get; set; } = string.Empty;

        [XmlElement("DOCUMENTNUMBER")]
        public string DocumentNumber { get; set; } = string.Empty;
        public bool ShouldSerializeDocumentNumber() => !string.IsNullOrEmpty(DocumentNumber);
    }
}
