namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    using System.Xml.Serialization;

    public class Profile
    {
        public Profile() { }

        [XmlElement("Customer")]
        public Customer Customer { get; set; } = new();

        [XmlAttribute("ProfileType")]
        public string ProfileType { get; set; } = "1";
    }
}
