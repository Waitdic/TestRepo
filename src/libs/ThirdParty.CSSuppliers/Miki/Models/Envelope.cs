namespace ThirdParty.CSSuppliers.Miki.Models
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = SoapNamespaces.Soap, ElementName = "Envelope")]
    public class Envelope<T> where T : SoapContent, new()
    {
        public SoapHeader Header { get; set; } = new();
        public SoapBody Body { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns
        {
            get => SoapNamespaces.Namespaces;
            set => throw new NotSupportedException();
        }

        [XmlType(Namespace = SoapNamespaces.Soap)]
        public class SoapHeader { }

        [XmlType(Namespace = SoapNamespaces.Soap)]
        public class SoapBody
        {
            [XmlElement(typeof(HotelSearchRequest), Namespace = SoapNamespaces.Ns, ElementName = "hotelSearchRequest")]
            [XmlElement(typeof(HotelSearchResponse), Namespace = SoapNamespaces.Ns, ElementName = "hotelSearchResponse")]
            [XmlElement(typeof(HotelBookingRequest), Namespace = SoapNamespaces.Ns, ElementName = "hotelBookingRequest")]
            [XmlElement(typeof(HotelBookingResponse), Namespace = SoapNamespaces.Ns, ElementName = "hotelBookingResponse")]
            [XmlElement(typeof(CancellationRequest), Namespace = SoapNamespaces.Ns, ElementName = "cancellationRequest")]
            [XmlElement(typeof(CancellationResponse), Namespace = SoapNamespaces.Ns, ElementName = "cancellationResponse")]
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
