namespace iVectorOne.CSSuppliers.OceanBeds.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Property
    {
        public string Id { get; set; } = string.Empty;

        public string PropertyCode { get; set; } = string.Empty;

        public string BoardBasis { get; set; } = string.Empty;

        public string CancellationText { get; set; } = string.Empty;

        [XmlArray("RoomList")]
        [XmlArrayItem("Room")]
        public Room[] RoomList { get; set; } = Array.Empty<Room>();
    }
}
