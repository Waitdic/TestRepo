namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Common
{
    using System.Xml.Serialization;

    public class DataElementsIndiv
    {
        [XmlElement("elementManagementData")]
        public ElementManagementData ElementManagementData { get; set; } = new();

        [XmlElement("ticketElement")]
        public TicketElement TicketElement { get; set; } = new();

        [XmlElement("freetextData")]
        public FreetextData FreetextData { get; set; } = new();
    }
}
