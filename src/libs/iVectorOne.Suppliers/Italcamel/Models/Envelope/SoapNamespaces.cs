namespace iVectorOne.Suppliers.Italcamel.Models.Envelope
{
    using System.Xml;
    using System.Xml.Serialization;

    public static class SoapNamespaces
    {
        public const string Soapenv = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string Ser = "http://webservice.italcamel.com/service_3_3.asmx";
        public const string Ns = "";

        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]{
                    new("soapenv", Soapenv),
                    new("ser", Ser),
                    new(string.Empty, Ns),
                });
        }
    }
}
