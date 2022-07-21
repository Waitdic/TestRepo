namespace iVectorOne.Suppliers.AmadeusHotels.Models.Header
{
    using System.Xml.Serialization;

    public class Password
    {
        [XmlAttribute("Type")]
        public string Type { get; set; } = string.Empty;

        [XmlText]
        public string DigestedPassword { get; set; } = string.Empty;
    }
}
