namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class SearchRoom
    {
        [XmlElement("ADULTS")]
        public int Adults { get; set; }

        [XmlElement("CHILDREN")]
        public int Children { get; set; }

        [XmlElement("CHILDAGE1")]
        public int ChildAge1 { get; set; }

        [XmlElement("CHILDAGE2")]
        public int ChildAge2 { get; set; }

        [XmlArray("ROOMDETAILS")]
        [XmlArrayItem("ROOMDETAIL")]
        public RoomDetail[] RoomDetails { get; set; } = Array.Empty<RoomDetail>();
        
        [XmlElement("NAME")]
        public string Name { get; set; } = string.Empty;
        public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name);

        [XmlElement("AVAILABLE")]
        public bool Available { get; set; }
        public bool ShouldSerializeAvailable() => Available;

        public string UID { get; set; } = string.Empty;
        public bool ShouldSerializeUID() => !string.IsNullOrEmpty(UID);

        public string HlpChildAgeCSV { get; set; } = string.Empty;
        public bool ShouldSerializeHlpChildAgeCSV() => !string.IsNullOrEmpty(HlpChildAgeCSV);
    }
}
