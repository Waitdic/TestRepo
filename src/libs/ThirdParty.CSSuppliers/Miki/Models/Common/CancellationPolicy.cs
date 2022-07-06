namespace ThirdParty.CSSuppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class CancellationPolicy
    {
        [XmlElement("fullStay")]
        public bool FullStay { get; set; }

        [XmlElement("appliesFrom")]
        public DateTime AppliesFrom { get; set; }

        [XmlElement("cancellationCharge")]
        public CancellationCharge CancellationCharge { get; set; } = new();
    }
}
