namespace iVectorOne.Suppliers.AbreuV2.Models
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public abstract class SoapContent { }

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
        public class SoapHeader
        {
            [XmlElement(Namespace = SoapNamespaces.Wsse)]
            public Security Security = new();
        }

        [XmlType(Namespace = SoapNamespaces.Soap)]
        public class SoapBody
        {
            [XmlElement(typeof(OTA_HotelAvailRQ), Namespace = SoapNamespaces.Ota, ElementName = "OTA_HotelAvailRQ")]
            [XmlElement(typeof(OTA_HotelResRQ), Namespace = SoapNamespaces.Ota, ElementName = "OTA_HotelResRQ")]
            [XmlElement(typeof(OTA_CancelRQ), Namespace = SoapNamespaces.Ota, ElementName = "OTA_CancelRQ")]
            public SoapContent SoapContent { get; set; } = new T();

            [XmlIgnore]
            public T Content
            {
                get => (T)SoapContent;
                set => SoapContent = value;
            }
        }
    }
    public static class SoapNamespaces
    {
        public const string Ota = "http://parsec.es/hotelapi/OTA2014Compact";
        public const string Wsse = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        public const string Soap = "http://schemas.xmlsoap.org/soap/envelope/";

        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]{
                    new("soap-env", Soap),
                });
        }
    }
}
