namespace iVectorOne.Suppliers.Models.Altura
{
    using System.Xml.Serialization;

    [XmlRoot("AlturaDS_responses")]
    public class AlturaSearchResponses
    {
        public AlturaSearchResponses() { }

        [XmlElement("Response")]
        public SearchResponse Response { get; set; } = new();
        public int PropertyRoomBookingID { get; set; }
    }
}
