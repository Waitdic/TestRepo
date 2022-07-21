namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class RoomCategory
    {
        public RoomCategory() { }

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;
    }
}