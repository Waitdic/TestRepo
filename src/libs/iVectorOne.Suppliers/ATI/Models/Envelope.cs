namespace ThirdParty.CSSuppliers.ATI.Models
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
            [XmlElement(typeof(AtiAvailabilityRequest), Namespace = SoapNamespaces.Ns1, ElementName = "OTA_HotelAvailRQ")]
            [XmlElement(typeof(AtiAvailabilitySearch), Namespace = SoapNamespaces.Ns1, ElementName = "OTA_HotelAvailRS")]
            [XmlElement(typeof(AtiBookRequest), Namespace = SoapNamespaces.Ns1, ElementName = "OTA_HotelResRQ")]
            [XmlElement(typeof(AtiBookResponse), Namespace = SoapNamespaces.Ns1, ElementName = "OTA_HotelResRS")]
            [XmlElement(typeof(AtiCancellationRequest), Namespace = SoapNamespaces.Ns1, ElementName = "OTA_CancelRQ")]
            [XmlElement(typeof(AtiCancellationResponse), Namespace = SoapNamespaces.Ns1, ElementName = "OTA_CancelRS")]
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
