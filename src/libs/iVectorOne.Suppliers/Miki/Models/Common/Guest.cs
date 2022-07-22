namespace iVectorOne.CSSuppliers.Miki.Models.Common
{
    using System.Xml.Serialization;

    public class Guest
    {
        [XmlElement("type")]
        public GuestCountType Type { get; set; }

        [XmlElement("age")]
        public int Age { get; set; }
        public bool ShouldSerializeAge() => Age != 0;

        [XmlElement("paxName", IsNullable = true)]
        public PaxName? PaxName { get; set; }
        public bool ShouldSerializePaxName() => PaxName != null;
    }
}
