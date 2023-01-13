namespace iVectorOne.Suppliers.PremierInn.Models.Soap
{
    using System.Xml;
    using System.Xml.Serialization;

    public class SoapNamespacesRequest
    {
        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespacesRequest()
        {
            Namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("soapenv", Constants.Soap),
                new("red", Constants.Red)
            });
        }
    }

    public class SoapNamespacesResponse
    {
        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespacesResponse()
        {
            Namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("SOAP-ENV", Constants.Soap),
                new("s", Constants.S)
            });
        }
    }
}
