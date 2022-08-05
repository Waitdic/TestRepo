namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class BookInsertRq : Main
    {
        public string AgentReference { get; set; } = string.Empty;
        public bool ShouldSerializeAgentReference() => !string.IsNullOrEmpty(AgentReference);

        public string HotelSearchCode { get; set; } = string.Empty;
        public string ArrivalDate { get; set; } = string.Empty;
        public int Nights { get; set; }
        public string NoAlternativeHotel { get; set; } = "1";
        public Leader Leader { get; set; } = new();

        [XmlArray("Rooms")]
        [XmlArrayItem("RoomType")]
        public List<RoomType> Rooms { get; set; } = new();

        public string Remark { get; set; } = string.Empty;
        public bool ShouldSerializeRemark() => !string.IsNullOrEmpty(Remark);
    }
}
