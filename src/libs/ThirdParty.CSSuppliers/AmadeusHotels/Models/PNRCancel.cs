namespace ThirdParty.CSSuppliers.AmadeusHotels.Models
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using Common;
    using Soap;

    public class PNRCancel : SoapContent
    {
        [XmlElement("reservationInfo")]
        public ReservationInfo ReservationInfo { get; set; } = new();

        [XmlArray("pnrActions")]
        [XmlArrayItem("optionCode")]
        public int[] PnrActions { get; set; } = Array.Empty<int>();

        [XmlElement("cancelElements")]
        public CancelElements CancelElements { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public PNRCancel()
        {
            Xmlns = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new(null, "http://xml.amadeus.com/PNRXCL_14_1_1A")
            });
        }
    }
}
