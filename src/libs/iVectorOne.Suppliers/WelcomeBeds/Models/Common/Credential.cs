namespace iVectorOne.Suppliers.Models.WelcomeBeds
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
