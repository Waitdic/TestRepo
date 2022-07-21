namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class RoomDescription
    {
        public RoomDescription() { }
        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;
    }

}
