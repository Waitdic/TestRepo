namespace iVectorOne.Suppliers.PremierInn.Models.Soap
{
    using System;
    using System.Xml.Serialization;
    using Search;
    using Book;
    using Cancel;

    [XmlRoot(Namespace = Constants.Soap, ElementName = "Envelope")]
    public class EnvelopeRequest<T> where T : SoapContent, new()
    {
        public SoapHeader Header { get; set; } = new();
        public SoapBody Body { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns
        {
            get => SoapNamespacesRequest.Namespaces;
            set => throw new NotSupportedException();
        }

        [XmlType(Namespace = Constants.Soap)]
        public class SoapBody
        {
            [XmlElement(typeof(ProcessMessage), Namespace = Constants.Red, ElementName = "ProcessMessage")]
            public SoapContent SoapContent { get; set; } = new T();

            [XmlIgnore]
            public T Content
            {
                get => (T)SoapContent;
                set => SoapContent = value;
            }
        }

        public class SoapHeader
        {
        }
    }

    [XmlRoot(ElementName = "Envelope")]
    public class EnvelopeResponse<T> where T : SoapContent, new()
    {
        public SoapBody Body { get; set; } = new();

        public class SoapBody
        {
            public MessageResponse ProcessMessageResponse { get; set; } = new();

            public class MessageResponse
            {
                [XmlElement(typeof(AvailabilityResponse), ElementName = "AvailabilityResponse")]
                [XmlElement(typeof(AvailabilityUpdateResponse), ElementName = "AvailabilityUpdateResponse")]
                [XmlElement(typeof(BookingConfirmResponse), ElementName = "BookingConfirmResponse")]
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
}
