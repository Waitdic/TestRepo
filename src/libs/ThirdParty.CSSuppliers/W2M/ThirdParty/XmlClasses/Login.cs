using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "Login")]
    public class Login
    {
        public Login(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public Login()
        {
        }

        [XmlAttribute(AttributeName = "Email")]
        public string Email { get; set; }

        [XmlAttribute(AttributeName = "Password")]
        public string Password { get; set; }
    }
}
