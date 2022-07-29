namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class StayDateRange
    {
        [XmlAttribute("End")]
        public string End { get; set; } = string.Empty;

        [XmlAttribute("Start")]
        public string Start { get; set; } = string.Empty;
    }
}
