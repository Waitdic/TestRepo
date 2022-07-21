namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlType(Namespace = SoapNamespaces.Wsa)]
    public class SoapHeader
    {
        [XmlElement("Credentials", Namespace = SoapNamespaces.Hot)]
        public Credentials Credentials { get; set; } = new();

        public string Action { get; set; } = string.Empty;

        public string To { get; set; } = string.Empty;
    }
}
