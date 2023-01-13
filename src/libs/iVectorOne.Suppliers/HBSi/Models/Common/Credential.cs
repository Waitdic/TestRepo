namespace iVectorOne.Suppliers.HBSi.Models
{
    using System.Xml.Serialization;

    public class Credential
    {
        public Credential() { }

        [XmlAttribute("CredentialCode")]
        public string CredentialCode { get; set; } = string.Empty;

        [XmlAttribute("CredentialName")]
        public string CredentialName { get; set; } = string.Empty;
    }
}
