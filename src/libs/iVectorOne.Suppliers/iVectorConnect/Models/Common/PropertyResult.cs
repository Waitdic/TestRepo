namespace ThirdParty.CSSuppliers.iVectorConnect.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PropertyResult
    {
        public string BookingToken { get; set; } = string.Empty;
        public int PropertyReferenceID { get; set; }
        public int GeographyLevel1ID { get; set; }
        public int GeographyLevel2ID { get; set; }
        public int GeographyLevel3ID { get; set; }

        [XmlArray("RoomTypes")]
        [XmlArrayItem("RoomType")]
        public List<RoomType> RoomTypes { get; set; } = new();
    }
}
