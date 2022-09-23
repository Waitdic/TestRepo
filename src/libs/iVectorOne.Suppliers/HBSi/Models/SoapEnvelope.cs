namespace iVectorOne.Suppliers.HBSi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;
    using Intuitive.Helpers.Serialization;

    public abstract class SoapContent
    {

        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public List<ResponseError> Errors { get; set; } = new();
        public bool ShouldSerializeErrors() => false;

        [XmlArray("Warnings")]
        [XmlArrayItem("Warning")]
        public List<Warning> Warnings { get; set; } = new();
        public bool ShouldSerializeWarnings() => false;


        [XmlElement("Success")]
        public string Success { get; set; } = "not empty";
        public bool ShouldSerializeSuccess() => false;

        public bool IsSuccess() => string.IsNullOrEmpty(Success);
    }


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

        [XmlType(Namespace = "http://www.hbsiapi.com/Documentation/XML/OTA/4/2005A/")]
        public class SoapHeader
        {
            [XmlElement("Interface")]
            public Interface Interface { get; set; } = new();
        }

        public class Interface
        {
            [XmlAttribute("ChannelIdentifierId")]
            public string ChannelIdentifierId { get; set; } = string.Empty;

            [XmlAttribute("Version")]
            public string Version { get; set; } = string.Empty;

            [XmlAttribute("Interface")]
            public string InterfaceAttr { get; set; } = string.Empty;

            [XmlElement("ComponentInfo")]
            public ComponentInfo ComponentInfo { get; set; } = new();
        }

        public class ComponentInfo
        {
            [XmlAttribute("Id")]
            public string Id { get; set; } = string.Empty;

            [XmlAttribute("User")]
            public string User { get; set; } = string.Empty;

            [XmlAttribute("Pwd")]
            public string Pwd { get; set; } = string.Empty;

            [XmlAttribute("ComponentType")]
            public string ComponentType { get; set; } = string.Empty;
        }

        [XmlType(Namespace = SoapNamespaces.Soap)]
        public class SoapBody
        {
            [XmlElement(typeof(OtaHotelAvailRq), Namespace = SoapNamespaces.Ns0, ElementName = "OTA_HotelAvailRQ")]
            [XmlElement(typeof(OtaHotelAvailRs), Namespace = SoapNamespaces.Ns0, ElementName = "OTA_HotelAvailRS")]
            [XmlElement(typeof(OtaHotelResRq), Namespace = SoapNamespaces.Ns0, ElementName = "OTA_HotelResRQ")]
            [XmlElement(typeof(OtaHotelResRs), Namespace = SoapNamespaces.Ns0, ElementName = "OTA_HotelResRS")]
            [XmlElement(typeof(OtaCancelRq), Namespace = SoapNamespaces.Ns0, ElementName = "OTA_CancelRQ")]
            [XmlElement(typeof(OtaCancelRs), Namespace = SoapNamespaces.Ns0, ElementName = "OTA_CancelRS")]
            [XmlElement(typeof(OtaReadRq), Namespace = SoapNamespaces.Ns0, ElementName = "OTA_ReadRQ")]
            [XmlElement(typeof(OtaResRetrieveRs), Namespace = SoapNamespaces.Ns0, ElementName = "OTA_ResRetrieveRS")]
            public SoapContent SoapContent { get; set; } = new T();

            [XmlIgnore]
            public T Content
            {
                get => (T)SoapContent;
                set => SoapContent = value;
            }

            [XmlAttribute("RequestId")]
            public string RequestId { get; set; } = string.Empty;

            [XmlAttribute("Transaction")]
            public string Transaction { get; set; } = string.Empty;
        }

        public static XmlDocument Serialize(ISerializer serializer, T envelopBodyContent, IHBSiSettings settings, IThirdPartyAttributeSearch tpAttributeSearch, 
                    string id, string transaction, string requestId, string source)
        {
            var soapEnvelope = new Envelope<T>
            {
                Header =
                {
                    Interface =
                    {
                        ChannelIdentifierId = settings.SalesChannel(tpAttributeSearch, source),
                        Version = "2005A",
                        InterfaceAttr = "HBSI XML 4 OTA",
                        ComponentInfo =
                        {
                            Id = id,
                            User = settings.User(tpAttributeSearch, source),
                            Pwd = settings.Password(tpAttributeSearch, source),
                            ComponentType = "Hotel"
                        }
                    }
                },
                Body =
                {
                    Transaction = transaction,
                    RequestId = requestId,
                    Content = envelopBodyContent
                }
            };
            return serializer.Serialize(soapEnvelope);
        }

        public static T DeSerialize(ISerializer serializer, XmlDocument xmlBody)
        {
            var envelope = serializer.DeSerialize<Envelope<T>>(xmlBody);
            return envelope.Body.Content;
        }
    }

    public static class SoapNamespaces
    {
        public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string Xsd = "http://www.w3.org/2001/XMLSchema";
        public const string Soap = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string Ns0 = "http://www.opentravel.org/OTA/2003/05";

        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]{
                    new("xsi", Xsi),
                    new("xsd", Xsd),
                    new("", Ns0),
                    new("soap-env", Soap),
                });
        }
    }
}
