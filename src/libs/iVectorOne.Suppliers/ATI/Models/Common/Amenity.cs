namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System.Xml.Serialization;

    public class Amenity
    {
        [XmlAttribute]
        public string CodeDetail { get; set; } = string.Empty;

        [XmlAttribute]
        public int RoomAmenityCode { get; set; }
    }
}