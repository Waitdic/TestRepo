namespace iVectorOne.CSSuppliers.AmadeusHotels.Models
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using Common;
    using Soap;

    public class HotelSell : SoapContent
    {
        [XmlElement("travelAgentRef")]
        public TravelAgentRef TravelAgentRef { get; set; } = new();

        [XmlElement("roomStayData")]
        public RoomStayData[] RoomStayData { get; set; } = Array.Empty<RoomStayData>();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public HotelSell()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new(null, "http://xml.amadeus.com/HBKRCQ_15_4_1A")
            });
        }
    }
}
