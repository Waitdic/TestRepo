namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class Guest
    {
        public Guest() { }

        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("Surname")]
        public string Surname { get; set; } = string.Empty;

        [XmlAttribute("Age")]
        public string Age { get; set; } = string.Empty;
    }
}