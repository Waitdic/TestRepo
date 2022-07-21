namespace ThirdParty.CSSuppliers.Hotelston
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Intuitive.Helpers.Extensions;
    using Intuitive.Helpers.Serialization;
    using ThirdParty.CSSuppliers.Hotelston.Models.Common;

    public static class HotelstonHelper
    {
        public const string DateFormatString = "yyyy-MM-dd";

        public static Envelope<T> CreateEnvelope<T>(IHotelstonSettings settings, IThirdPartyAttributeSearch tpAttributeSearch) where T : RequestBase, new()
        {
            return new Envelope<T>(settings.SoapRequestUrl(tpAttributeSearch), settings.SoapTypesUrl(tpAttributeSearch))
            {
                Body =
                {
                    Content =
                    {
                        Locale = settings.LanguageCode(tpAttributeSearch),
                        LoginDetails =
                        {
                            Email = settings.Email(tpAttributeSearch),
                            Password = settings.Password(tpAttributeSearch),
                        },
                        Currency = settings.Currency(tpAttributeSearch),
                        NetRates = settings.NetRates(tpAttributeSearch).ToSafeString().ToLower(),
                    }
                }
            };
        }

        public static XmlDocument Serialize(object envelope, XmlSerializerNamespaces namespaces, XmlAttributeOverrides attributeOverrides)
        {
            // Custom Serialize method because Intuitive.Serializer.Serialize does not allow overriding default prefixes
            // as a result, when trying to redefine the xsd prefix, an error occurs:
            // System.InvalidOperationException : Illegal namespace declaration xmlns:xsd='http://request.ws.hotelston.com/xsd'.
            // Namespace alias 'xsd' already defined in the current scope.

            var serializer = new XmlSerializer(envelope.GetType(), attributeOverrides); // support dynamic namespaces
            var builder = new StringBuilder();

            using (var sw = new StringWriter(builder))
            using (var writer = new CustomXmlWriter(sw, namespaces))
            {
                // in fact, this entire custom serializer is needed only because of the third parameter,
                // the full list of prefixes is passed here, and not just additional prefixes 
                serializer.Serialize(writer, envelope, namespaces);
            }

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(builder.ToString());
            return xmlDocument;
        }

        public static T DeSerialize<T>(XmlDocument xml, ISerializer serializer) where T : SoapContent
        {
            return serializer.DeSerialize<T>(serializer.CleanSoapDocument(xml.InnerXml));
        }

        /// <summary>
        /// Custom Xml Writer adds a namespace prefix to all attributes
        /// if they don't have their own prefix 
        /// </summary>
        private class CustomXmlWriter : XmlTextWriter
        {
            private Dictionary<string, XmlQualifiedName> NamespaceLookup { get; }

            private string CurrentElementPrefix { get; set; } = string.Empty;

            public override void WriteStartDocument()
            {
                // Omit Xml Declaration
            }

            public CustomXmlWriter(TextWriter writer, XmlSerializerNamespaces namespaces) : base(writer)
            {
                NamespaceLookup = namespaces.ToArray().ToDictionary(n => n.Namespace);
            }

            public override void WriteStartAttribute(string prefix, string localName, string? ns)
            {
                if (string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(ns))
                {
                    // each attribute in the request must be prefixed, this is not in accordance with the XML standard,
                    // (because attribute prefix is omitted if equal to element prefix),
                    // but otherwise the suppler does not see the attributes and returns an error 
                    prefix = CurrentElementPrefix;
                    ns = null;
                }

                base.WriteStartAttribute(prefix, localName, ns!);
            }

            public override void WriteStartElement(string prefix, string localName, string ns)
            {
                if (string.IsNullOrEmpty(prefix) && NamespaceLookup.TryGetValue(ns, out var qualifiedName))
                {
                    CurrentElementPrefix = qualifiedName.Name;
                }
                else
                {
                    CurrentElementPrefix = prefix;
                }

                base.WriteStartElement(prefix, localName, ns);
            }
        }
    }
}
