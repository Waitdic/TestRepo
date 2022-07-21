namespace ThirdParty.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class OccupancyInfo
    {
        public int Duration { get; set; }

        [XmlElement("Room")]
        public Room[] Rooms { get; set; } = Array.Empty<Room>();
    }
}
