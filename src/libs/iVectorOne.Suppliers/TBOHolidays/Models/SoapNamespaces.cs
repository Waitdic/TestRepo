namespace iVectorOne.CSSuppliers.TBOHolidays.Models
{
    using System.Xml;
    using System.Xml.Serialization;

    public class SoapNamespaces
    {
        public const string Soap = "http://www.w3.org/2003/05/soap-envelope";
        public const string Hot = "http://TekTravel/HotelBookingApi";
        public const string Wsa = "http://www.w3.org/2005/08/addressing";

        public static XmlSerializerNamespaces Namespaces { get; }

        static SoapNamespaces()
        {
            Namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[]
            {
                new("soap", Soap),
                new("hot", Hot),
                new("wsa", Wsa)
            });
        }
    }
}
