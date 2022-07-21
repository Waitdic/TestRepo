namespace iVectorOne.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Property
    {
        public string TPKey { get; set; }
        [XmlAttribute("PropertyCode")]
        public string PropertyCode { get; set; } = string.Empty;

        [XmlArray("RoomTypes")]
        [XmlArrayItem("RoomType")]
        public RoomType[] RoomTypes { get; set; } = Array.Empty<RoomType>();
    }
}
