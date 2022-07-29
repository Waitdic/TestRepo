namespace iVectorOne.Suppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class BasicPropertyInfo
    {
        [XmlAttribute("HotelCode")]
        public string HotelCode { get; set; } = string.Empty;

        [XmlArray("Comments")]
        [XmlArrayItem("Comment")]
        public Comment[] Comments { get; set; } = Array.Empty<Comment>();
    }
}
