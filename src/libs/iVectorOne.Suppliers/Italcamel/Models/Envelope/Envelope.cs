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
            [XmlElement(typeof(GetAvailabilitySplitted), Namespace = SoapNamespaces.Ser, ElementName = "GETAVAILABILITYSPLITTED")]
            [XmlElement(typeof(GetAvailabilitySplittedResponse), Namespace = SoapNamespaces.Ser, ElementName = "GETAVAILABILITYSPLITTEDResponse")]
            [XmlElement(typeof(BookingEstimate), Namespace = SoapNamespaces.Ser, ElementName = "BOOKINGESTIMATE")]
            [XmlElement(typeof(BookingEstimateResponse), Namespace = SoapNamespaces.Ser, ElementName = "BOOKINGESTIMATEResponse")]
            [XmlElement(typeof(BookingDelete), Namespace = SoapNamespaces.Ser, ElementName = "BOOKINGESTIMATE")]
            [XmlElement(typeof(BookingDeleteResponse), Namespace = SoapNamespaces.Ser, ElementName = "BOOKINGDELETEResponse")]
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
