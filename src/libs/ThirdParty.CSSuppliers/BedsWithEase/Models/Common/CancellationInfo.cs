namespace ThirdParty.CSSuppliers.BedsWithEase.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class CancellationInfo
    {
        public bool Supported { get; set; }

        [XmlArray("CancellationNotes")]
        [XmlArrayItem("CancellationNote")]
        public CancellationNote[] CancellationNotes { get; set; } = Array.Empty<CancellationNote>();

        public string CancellationFeeAmount { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = string.Empty;

        [XmlArray("CancellationPolicies")]
        [XmlArrayItem("CancellationPolicy")]
        public CancellationPolicy[] CancellationPolicies { get; set; } = Array.Empty<CancellationPolicy>();
    }
}