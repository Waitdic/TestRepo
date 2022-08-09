namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using Intuitive;
    using Intuitive.Helpers.Serialization;

    [XmlRoot(Namespace = SoapNamespaces.Soap, ElementName = "Envelope")]
    public class Envelope
    {
        public SoapBody Body { get; set; } = new();


        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns
        {
            get => SoapNamespaces.Namespaces;
            set => throw new NotSupportedException();
        }

        [XmlType(Namespace = SoapNamespaces.Soap)]
        public class SoapBody
        {
            [XmlElement(Namespace = SoapNamespaces.GoGlb)]
            public MakeRequest MakeRequest { get; set; } = new();

            [XmlElement(Namespace = SoapNamespaces.GoGlb)]
            public MakeRequestResponse MakeRequestResponse { get; set; }

            public bool ShouldSerializeMakeRequestResponse() => MakeRequestResponse != null;
        }

        public static XmlDocument CreateRequest(int requestType, string sXmlRequest, ISerializer serializer)
        {
            var envelope = new Envelope
            {
                Body =
                {
                    MakeRequest =
                    {
                        RequestType = requestType,
                        XmlRequest =
                        {
                            Content = sXmlRequest
                        }
                    }
                }
            };

            return serializer.Serialize(envelope);
        }

        public static string GetResponse(XmlDocument xmlDocument, ISerializer serializer)
        {
            var envelope = serializer.DeSerialize<Envelope>(xmlDocument);
            string requestResult = envelope.Body.MakeRequestResponse.MakeRequestResult;
            return requestResult;
        }
    }

    public class MakeRequest
    {
        public MakeRequest()
        {
            xmlns.Add("", SoapNamespaces.GoGlb);
        }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns = new();

        [XmlElement("requestType")]
        public int RequestType { get; set; }

        [XmlElement("xmlRequest")]
        public XmlRequest XmlRequest { get; set; } = new();
    }

    public class XmlRequest
    {
        [XmlIgnore]
        public string Content { get; set; }

        [XmlText]
        public XmlNode[] CDataContent
        {
            get
            {
                var dummy = new XmlDocument();
                return new XmlNode[] { dummy.CreateCDataSection(Content) };
            }
            set
            {
                Content = value[0].Value;
            }
        }
    }

    public class MakeRequestResponse
    {
        public string MakeRequestResult { get; set; } = string.Empty;
    }

    public static class SoapNamespaces
    {
        public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string Xsd = "http://www.w3.org/2001/XMLSchema";
        public const string Soap = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string GoGlb = "http://www.goglobal.travel/";

        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]{
                    new("soap", Soap),
                    new("xsi", Xsi),
                    new("xsd", Xsd),
                });
        }
    }
}
