namespace ThirdParty.CSSuppliers.AmadeusHotels.Models
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using Common;
    using Soap;

    public class PNRAddMultiElements : SoapContent
    {
        [XmlArray("pnrActions")]
        [XmlArrayItem("optionCode")]
        public int[] PnrActions { get; set; } = Array.Empty<int>();

        [XmlElement("travellerInfo")]
        public TravellerInfo TravellerInfo { get; set; } = new();

        [XmlElement("dataElementsMaster")]
        public DataElementsMaster DataElementsMaster { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public PNRAddMultiElements()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new(null, "http://xml.amadeus.com/PNRADD_14_1_1A")
            });
        }
    }
}
