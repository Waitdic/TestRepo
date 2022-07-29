namespace iVectorOne.Suppliers.Hotelston.Models.Common
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = Soap, ElementName = "Envelope")]
    public class Envelope<T> where T : SoapContent, new()
    {
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; set; }

        public const string Soap = "http://schemas.xmlsoap.org/soap/envelope/";

        [XmlIgnore]
        public string Xsd { get; private set; } = string.Empty;

        [XmlIgnore]
        public string Xsd1 { get; private set; } = string.Empty;

        [XmlIgnore]
        public XmlAttributeOverrides AttributeOverrides { get; set; }

        public Envelope()
        {
            Xmlns = new XmlSerializerNamespaces();
            AttributeOverrides = new XmlAttributeOverrides();
        }

        public Envelope(string soapRequestUrl, string soapTypesUrl)
        {
            Xsd = soapRequestUrl;
            Xsd1 = soapTypesUrl;

            Type[] types =
            {
                typeof(BookHotelRequest),
                typeof(BookingTermsRequest),
                typeof(CancelHotelBookingRequest),
                typeof(CheckAvailabilityRequest),
                typeof(SearchHotelsRequest),
            };

            Xmlns = new XmlSerializerNamespaces(
                new XmlQualifiedName[] { new("xsd1", Xsd1), new("xsd", Xsd), new("soapenv", Soap), });

            AttributeOverrides = new XmlAttributeOverrides();
            var attrs = new XmlAttributes();
            foreach (var type in types)
            {
                attrs.XmlElements.Add(new XmlElementAttribute { Namespace = Xsd, Type = type, ElementName = type.Name });
            }

            AttributeOverrides.Add(typeof(SoapBody), nameof(SoapContent), attrs);

            var emailAttrs = new XmlAttributes
            {
                XmlAttribute = new XmlAttributeAttribute
                {
                    Namespace = Xsd1,
                    AttributeName = nameof(LoginDetails.Email).ToLower()
                }
            };
            AttributeOverrides.Add(typeof(LoginDetails), nameof(LoginDetails.Email), emailAttrs);

            var passwordAttrs = new XmlAttributes
            {
                XmlAttribute = new XmlAttributeAttribute
                {
                    Namespace = Xsd1,
                    AttributeName = nameof(LoginDetails.Password).ToLower()
                }
            };
            AttributeOverrides.Add(typeof(LoginDetails), nameof(LoginDetails.Password), passwordAttrs);
        }

        public SoapHeader Header { get; set; } = new();

        public SoapBody Body { get; set; } = new();

        public class SoapHeader { }

        public class SoapBody
        {
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