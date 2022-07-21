namespace iVectorOne.CSSuppliers.BedsWithEase.Models.Common
{
    using System.Xml;
    using System.Xml.Serialization;

    public static class SoapNamespaces
    {
        public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string Xsd = "http://www.w3.org/2001/XMLSchema";
        public const string Soap = "http://schemas.xmlsoap.org/soap/envelope/";
        public const string BlackBox = "http://topdog.uk.net/BlackBox";

        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]{
                    new("xsi", Xsi),
                    new("xsd", Xsd),
                    new("soap", Soap),
                });
        }
    }
}