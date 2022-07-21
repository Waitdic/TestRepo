namespace iVectorOne.Suppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class Ticket
    {
        [XmlElement("indicator")]
        public string Indicator { get; set; } = string.Empty;
    }
}
