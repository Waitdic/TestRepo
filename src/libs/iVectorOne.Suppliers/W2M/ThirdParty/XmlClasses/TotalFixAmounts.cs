using System.Xml.Serialization;

namespace ThirdParty.CSSuppliers.Xml.W2M
{
#pragma warning disable CS8618
    [XmlRoot(ElementName = "TotalFixAmounts")]
    public class TotalFixAmounts
    {
        public TotalFixAmounts(Service service, string recommended, ServiceTaxes serviceTaxes,
            decimal gross, decimal nett)
        {
            Service = service;
            Recommended = recommended;
            ServiceTaxes = serviceTaxes;
            Gross = gross;
            Nett = nett;
        }

        public TotalFixAmounts()
        {
        }

        [XmlElement(ElementName = "Service")]
        public Service Service { get; set; }

        [XmlAttribute(AttributeName = "Recommended")]
        public string Recommended { get; set; }

        [XmlElement(ElementName = "ServiceTaxes")]
        public ServiceTaxes ServiceTaxes { get; set; }

        [XmlAttribute(AttributeName = "Gross")]
        public decimal Gross { get; set; }

        [XmlAttribute(AttributeName = "Nett")]
        public decimal Nett { get; set; }
    }
}
