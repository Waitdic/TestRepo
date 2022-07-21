namespace ThirdParty.CSSuppliers.Juniper.Model
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using Intuitive;
    using Intuitive.Helpers.Serialization;

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
        public class SoapHeader { }

        [XmlType(Namespace = SoapNamespaces.Soap)]
        public class SoapBody
        {
            [XmlElement(typeof(OTA_HotelAvailService), Namespace = SoapNamespaces.Ota, ElementName = "OTA_HotelAvailService")]
            public SoapContent SoapContent { get; set; } = new T();

            [XmlIgnore]
            public T Content
            {
                get => (T)SoapContent;
                set => SoapContent = value;
            }
        }

        public static XmlDocument Serialize(T envelopBodyContent, ISerializer serializer)
        {
            return serializer.Serialize(new Envelope<T> { Body = { Content = envelopBodyContent } });
        }

        public static T DeSerialize(XmlDocument xmlBody, ISerializer serializer)
        {
            var envelope = serializer.DeSerialize<Envelope<T>>(xmlBody);
            return envelope.Body.Content;
        }
    }
    public static class SoapNamespaces
    {
        public const string Ota = "http://www.opentravel.org/OTA/2003/05";
        public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string Xsd = "http://www.w3.org/2001/XMLSchema";
        public const string Soap = "http://schemas.xmlsoap.org/soap/envelope/";

        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]{
                    new("xsi", Xsi),
                    new("xsd", Xsd),
                    new("", Ota),
                    new("soapenv", Soap),
                });
        }
    }
}