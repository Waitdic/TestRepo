namespace ThirdParty.CSSuppliers.Models.WelcomeBeds
{
    using System.Xml.Serialization;

    public class RoomRateDescription
    {
        public RoomRateDescription() { }

        [XmlAttribute("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Text")]
        public string Text { get; set; } = string.Empty;
    }

}
