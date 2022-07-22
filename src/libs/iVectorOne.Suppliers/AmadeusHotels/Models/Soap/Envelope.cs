namespace iVectorOne.CSSuppliers.AmadeusHotels.Models.Soap
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using Header;

    [XmlRoot(Namespace = SoapNamespaces.Soap, ElementName = "Envelope")]
    public class Envelope<T> where T : SoapContent, new()
    {
        public SoapHeader Header { get; set; } = new();

        public SoapBody Body { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns
        {
            get => new(new XmlQualifiedName[]{ new("soap", SoapNamespaces.Soap)});
            set => throw new NotSupportedException();
        }

        [XmlType(Namespace = SoapNamespaces.Soap)]
        public class SoapBody
        {
            [XmlElement(typeof(OTAHotelAvailRQ), Namespace = SoapNamespaces.Ns, ElementName = "OTA_HotelAvailRQ")]
            [XmlElement(typeof(SecuritySignOut), Namespace = SoapNamespaces.Ns, ElementName = "Security_SignOut")]
            [XmlElement(typeof(PNRAddMultiElements), Namespace = SoapNamespaces.Ns, ElementName = "PNR_AddMultiElements")]
            [XmlElement(typeof(HotelSell), Namespace = SoapNamespaces.Ns, ElementName = "Hotel_Sell")]
            [XmlElement(typeof(PNRCancel), Namespace = SoapNamespaces.Ns, ElementName = "PNR_Cancel")]
            public SoapContent SoapContent { get; set; } = new T();

            [XmlIgnore]
            public T Content
            {
                get => (T)SoapContent;
                set => SoapContent = value;
            }
        }
    }
}
