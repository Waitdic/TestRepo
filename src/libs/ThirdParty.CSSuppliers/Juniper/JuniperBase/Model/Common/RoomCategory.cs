using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Model.JuniperBase
{
    public class RoomCategory
    {
        public RoomCategory() { }

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;
    }
}
