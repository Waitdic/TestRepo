namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Accommodation
    {
        [XmlArray("RoomProfiles")]
        [XmlArrayItem("RoomProfile")]
        public RoomProfile[] RoomProfiles { get; set; } = Array.Empty<RoomProfile>();
    }
}
