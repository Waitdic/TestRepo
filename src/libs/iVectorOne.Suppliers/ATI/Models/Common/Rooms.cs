namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot("Rooms")]
    public class Rooms
    {
        [XmlElement("Room")]
        public Room[] Room { get; set; } = Array.Empty<Room>();
    }
}
