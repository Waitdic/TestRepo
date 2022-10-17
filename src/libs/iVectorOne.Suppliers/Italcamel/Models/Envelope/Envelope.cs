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
        public SoapHeader Header { get; set; } = new();
        public SoapBody Body { get; set; } = new();

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns
        {
            get => SoapNamespaces.Namespaces;
            set => throw new NotSupportedException();
        }

        [XmlType(Namespace = SoapNamespaces.Soapenv)]
        public class SoapHeader { }

        [XmlType(Namespace = SoapNamespaces.Soapenv)]
        public class SoapBody
        {
            [XmlElement(typeof(GetAvailability), Namespace = SoapNamespaces.Ns, ElementName = "GETAVAILABILITY")]
            [XmlElement(typeof(GetAvailabilityResponse), Namespace = SoapNamespaces.Ns, ElementName = "GETAVAILABILITYResponse")]
            [XmlElement(typeof(PackageEstimate), Namespace = SoapNamespaces.Ns, ElementName = "PACKAGEESTIMATE")]
            [XmlElement(typeof(PackageEstimateResponse), Namespace = SoapNamespaces.Ns, ElementName = "PACKAGEESTIMATEResponse")]
            [XmlElement(typeof(PackageDelete), Namespace = SoapNamespaces.Ns, ElementName = "PACKAGEDELETE")]
            [XmlElement(typeof(PackageDeleteResponse), Namespace = SoapNamespaces.Ns, ElementName = "PACKAGEDELETEResponse")]
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
