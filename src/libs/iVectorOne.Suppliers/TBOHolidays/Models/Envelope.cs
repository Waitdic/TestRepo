namespace ThirdParty.CSSuppliers.TBOHolidays.Models
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
        public class SoapBody
        {
            [XmlElement(typeof(AvailabilityAndPricingRequest), Namespace = SoapNamespaces.Hot, ElementName = "AvailabilityAndPricingRequest")]
            [XmlElement(typeof(AvailabilityAndPricingResponse), Namespace = SoapNamespaces.Hot, ElementName = "AvailabilityAndPricingResponse")]
            [XmlElement(typeof(HotelBookRequest), Namespace = SoapNamespaces.Hot, ElementName = "HotelBookRequest")]
            [XmlElement(typeof(HotelBookResponse), Namespace = SoapNamespaces.Hot, ElementName = "HotelBookResponse")]
            [XmlElement(typeof(HotelCancellationPolicyRequest), Namespace = SoapNamespaces.Hot, ElementName = "HotelCancellationPolicyRequest")]
            [XmlElement(typeof(HotelCancellationPolicyResponse), Namespace = SoapNamespaces.Hot, ElementName = "HotelCancellationPolicyResponse")]
            [XmlElement(typeof(HotelCancelRequest), Namespace = SoapNamespaces.Hot, ElementName = "HotelCancelRequest")]
            [XmlElement(typeof(HotelCancelResponse), Namespace = SoapNamespaces.Hot, ElementName = "HotelCancelResponse")]
            [XmlElement(typeof(HotelSearchWithRoomsRequest), Namespace = SoapNamespaces.Hot, ElementName = "HotelSearchWithRoomsRequest")]
            [XmlElement(typeof(HotelSearchWithRoomsResponse), Namespace = SoapNamespaces.Hot, ElementName = "HotelSearchWithRoomsResponse")]
            [XmlElement(typeof(HotelBookingDetailRequest), Namespace = SoapNamespaces.Hot, ElementName = "HotelBookingDetailRequest")]
            [XmlElement(typeof(HotelBookingDetailResponse), Namespace = SoapNamespaces.Hot, ElementName = "HotelBookingDetailResponse")]
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
