namespace iVectorOne.CSSuppliers.TeamAmerica.Models
{
    using System.Xml.Serialization;

    public class NewPassenger
    {
        [XmlElement("Salutation")]
        public string Salutation { get; set; } = string.Empty;

        [XmlElement("FamilyName")]
        public string FamilyName { get; set; } = string.Empty;

        [XmlElement("FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [XmlElement("PassengerType")]
        public string PassengerType { get; set; } = string.Empty;

        [XmlElement("NationalityCode")]
        public string NationalityCode { get; set; } = string.Empty;

        [XmlElement("PassengerAge")]
        public int PassengerAge { get; set; }
    }
}