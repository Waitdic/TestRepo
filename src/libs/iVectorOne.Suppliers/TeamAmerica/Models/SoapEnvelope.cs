namespace iVectorOne.CSSuppliers.TeamAmerica.Models
{
    using System;
    using System.Text.RegularExpressions;
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
            [XmlElement(typeof(PriceSearch), Namespace = SoapNamespaces.Xsd, ElementName = "PriceSearch")]
            [XmlElement(typeof(NewMultiItemReservation), Namespace = SoapNamespaces.Xsd, ElementName = "NewMultiItemReservation")]
            [XmlElement(typeof(CancelReservation), Namespace = SoapNamespaces.Xsd, ElementName = "CancelReservation")]
            public SoapContent SoapContent { get; set; } = new T();

            [XmlIgnore]
            public T Content
            {
                get => (T)SoapContent;
                set => SoapContent = value;
            }
        }

        public static string Serialize(T envelopBodyContent, ISerializer serializer)
        {
            var xmlDoc = serializer.Serialize(new Envelope<T> { Body = { Content = envelopBodyContent } });
            var strDoc = ReplaceNs(xmlDoc.OuterXml, SoapNamespaces.XsdShadowPrefix, SoapNamespaces.XsdPrefix);

            return strDoc;
        }

        /// <summary>
        /// Unable to redefine xsd namespace while serialization
        /// To get around this behaviour
        /// Serialize with 'shadow' namespace then replace it with xsd namespace
        /// </summary>
        /// <param name="strDoc"></param>
        /// <param name="oldNs"></param>
        /// <param name="newNs"></param>
        /// <returns></returns>
        private static string ReplaceNs(string strDoc, string oldNs, string newNs)
        {

            strDoc = strDoc.Replace($"xmlns:{oldNs}=\"http://www.wso2.org/php/xsd\""
                                  , $"xmlns:{newNs}=\"http://www.wso2.org/php/xsd\"");

            strDoc = Regex.Replace(strDoc, $"<{oldNs}:(\\w+)>", match => $"<{newNs}:{match.Groups[1].Value}>");
            strDoc = Regex.Replace(strDoc, $"</{oldNs}:(\\w+)>", match => $"</{newNs}:{match.Groups[1].Value}>");
            strDoc = Regex.Replace(strDoc, $"<{oldNs}:(\\w+) />", match => $"<{newNs}:{match.Groups[1].Value} />");

            return strDoc;
        }

        public static T DeSerialize(XmlDocument soapEvelope, ISerializer serializer)
        {
            var xmlClear = serializer.CleanXmlNamespaces(soapEvelope);
            var body = xmlClear.SelectSingleNode("/Envelope/Body").InnerXml;
            var result = serializer.DeSerialize<T>(body);
            return result;
        }
    }

    public static class SoapNamespaces
    {
        public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string Xsd = "http://www.wso2.org/php/xsd";
        public const string Soap = "http://schemas.xmlsoap.org/soap/envelope/";

        public const string XsdPrefix = "xsd";
        public const string XsdShadowPrefix = "qq99";


        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]{
                    new(XsdShadowPrefix, Xsd),
                    new("soapenv", Soap)
                });
        }
    }
}
