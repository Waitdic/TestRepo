namespace iVectorOne.CSSuppliers.Miki.Models
{
    using System.Xml;
    using System.Xml.Serialization;

    public class SoapNamespaces
    {
        public const string Soap = "http://www.w3.org/2003/05/soap-envelope";
        public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string Ns = "";

        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(
                new XmlQualifiedName[]{
                    new("soap", Soap),
                    new("xsi", Xsi),
                });
        }
    }
}
