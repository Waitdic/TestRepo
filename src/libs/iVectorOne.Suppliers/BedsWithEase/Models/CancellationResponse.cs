namespace iVectorOne.CSSuppliers.BedsWithEase.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class CancellationResponse : SoapContent
    {
        [XmlElement("Cancelled")]
        public Cancelled Cancelled = new();

        [XmlArray("Errors")]
        [XmlArrayItem("Error")]
        public Error[] Errors { get; set; } = Array.Empty<Error>();
    }

    public class Cancelled
    {
        public string SupplierCclStatus { get; set; } = string.Empty;

        public string Price { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = string.Empty;
    }
}
