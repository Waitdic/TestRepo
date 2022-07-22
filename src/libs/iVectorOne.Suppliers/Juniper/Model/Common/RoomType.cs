namespace iVectorOne.CSSuppliers.Juniper.Model
{
    using System.Xml.Serialization;

    public class RoomType
    {
        public RoomType() { }

        [XmlElement("TPA_Extensions")]
        public RoomTypeExtension RoomTypeExtension { get; set; } = new();
    }
}