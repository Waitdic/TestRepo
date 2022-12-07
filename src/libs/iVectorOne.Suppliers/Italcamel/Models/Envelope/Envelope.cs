namespace iVectorOne.Suppliers.Italcamel.Models.Envelope
{
    using System;
    using System.Xml.Serialization;
    using iVectorOne.Suppliers.Italcamel.Models.Prebook;
    using iVectorOne.Suppliers.Italcamel.Models.Search;
    using iVectorOne.Suppliers.Italcamel.Models.Cancel;

    [XmlRoot(Namespace = SoapNamespaces.Soapenv, ElementName = "Envelope")]
    public class Envelope<T> where T : SoapContent, new()
    {
        public SoapBody Body { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns
        {
            get => SoapNamespaces.Namespaces;
            set => throw new NotSupportedException();
        }

        [XmlType(Namespace = SoapNamespaces.Soapenv)]
        public class SoapBody
        {
            [XmlElement(typeof(GetAvailability), Namespace = SoapNamespaces.Ser, ElementName = "GETAVAILABILITY")]
            [XmlElement(typeof(GetAvailabilityResponse), Namespace = SoapNamespaces.Ser, ElementName = "GETAVAILABILITYResponse")]
            [XmlElement(typeof(PackageEstimate), Namespace = SoapNamespaces.Ser, ElementName = "PACKAGEESTIMATE")]
            [XmlElement(typeof(PackageEstimateResponse), Namespace = SoapNamespaces.Ser, ElementName = "PACKAGEESTIMATEResponse")]
            [XmlElement(typeof(PackageDelete), Namespace = SoapNamespaces.Ser, ElementName = "PACKAGEDELETE")]
            [XmlElement(typeof(PackageDeleteResponse), Namespace = SoapNamespaces.Ser, ElementName = "PACKAGEDELETEResponse")]
            [XmlElement(typeof(GetBookingCharge), Namespace = SoapNamespaces.Ser, ElementName = "GETBOOKINGCHARGE")]
            [XmlElement(typeof(GetBookingChargeResponse), Namespace = SoapNamespaces.Ser, ElementName = "GETBOOKINGCHARGEResponse")]
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
