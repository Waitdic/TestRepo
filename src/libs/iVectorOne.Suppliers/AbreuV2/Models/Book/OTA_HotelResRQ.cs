using System.Collections.Generic;
using System.Xml.Serialization;

namespace iVectorOne.CSSuppliers.AbreuV2.Models
{
    public class OTA_HotelResRQ : SoapContent
    {
        [XmlAttribute("Transaction")]
        public string Transaction { get; set; } = string.Empty;
        public bool ShouldSerializeTransaction() => !string.IsNullOrEmpty(Transaction);

        [XmlAttribute("DetailLevel")]
        public string DetailLevel { get; set; } = string.Empty;
        public bool ShouldSerializeDetailLevel() => !string.IsNullOrEmpty(DetailLevel);

        [XmlElement("UniqueID")]
        public UniqueID UniqueID { get; set; } = new();
        public bool ShouldSerializeUniqueID() => !string.IsNullOrEmpty(UniqueID.ID);

        [XmlElement("HotelRes")]
        public HotelRes HotelRes { get; set; } = new();
    }

    public class HotelRes
    {
        [XmlArray("Rooms")]
        [XmlArrayItem("Room")]
        public List<RoomRq> Rooms { get; set; } = new();
    }
}
