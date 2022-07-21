namespace iVectorOne.Suppliers.BedsWithEase.Models.Common
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = SoapNamespaces.Soap, ElementName = "Envelope")]
    public class Envelope<T> where T : SoapContent, new()
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
            [XmlElement(typeof(CancellationInfoRequest), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(CancellationInfoRequest))]
            [XmlElement(typeof(CancellationInfoResponse), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(CancellationInfoResponse))]
            [XmlElement(typeof(ConfirmationRequest), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(ConfirmationRequest))]
            [XmlElement(typeof(ConfirmationResponse), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(ConfirmationResponse))]
            [XmlElement(typeof(EndSessionRequest), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(EndSessionRequest))]
            [XmlElement(typeof(HotelAvailabilityRequest), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(HotelAvailabilityRequest))]
            [XmlElement(typeof(HotelAvailabilityResponse), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(HotelAvailabilityResponse))]
            [XmlElement(typeof(HotelReservationRequest), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(HotelReservationRequest))]
            [XmlElement(typeof(HotelReservationResponse), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(HotelReservationResponse))]
            [XmlElement(typeof(StartSessionRequest), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(StartSessionRequest))]
            [XmlElement(typeof(StartSessionResponse), Namespace = SoapNamespaces.BlackBox, ElementName = nameof(StartSessionResponse))]
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