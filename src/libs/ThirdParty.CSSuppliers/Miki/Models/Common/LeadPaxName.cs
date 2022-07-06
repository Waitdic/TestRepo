namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class LeadPaxName
    {
        [XmlElement("title")]
        public string Title { get; set; } = string.Empty;

        [XmlElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [XmlElement("lastName")]
        public string LastName { get; set; } = string.Empty;
    }
}
