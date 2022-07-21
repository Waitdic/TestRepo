namespace iVectorOne.Suppliers.ATI.Models
{
    using System.Xml;
    using System.Xml.Serialization;

    public static class SoapNamespaces
    {
        public const string Soap = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string Ns1 = "http://www.opentravel.org/OTA/2003/05";
        public const string Ns = "";

        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]{
                    new("soap", Soap),
                    new("ns1", Ns1),
                    new(string.Empty, Ns),
                });
        }
    }
}
