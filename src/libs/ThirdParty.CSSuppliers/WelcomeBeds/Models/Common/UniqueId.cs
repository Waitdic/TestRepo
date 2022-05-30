namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class UniqueId
    {
        public UniqueId() { }

        [XmlAttribute("ID")]
        public string Id { get; set; } = string.Empty;
    }
}
