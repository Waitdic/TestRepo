namespace ThirdParty.CSSuppliers.iVectorConnect.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("PropertyPreBookResponse")]
    public class PropertyPreBookResponse
    {
        public ReturnStatus ReturnStatus { get; set; } = new();

        public string BookingToken { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        [XmlArray("Cancellations")]
        [XmlArrayItem("Cancellation")]
        public Cancellation[] Cancellations { get; set; } = Array.Empty<Cancellation>();

        [XmlArray("Errata")]
        [XmlArrayItem("Erratum")]
        public Erratum[] Errata { get; set; } = Array.Empty<Erratum>();
    }
}
