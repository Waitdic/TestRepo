namespace iVectorOne.CSSuppliers.Hotelston.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class CancellationPolicy
    {
        [XmlElement("cancellationRule")]
        public CancellationRule[] CancellationRules { get; set; } = Array.Empty<CancellationRule>();
    }
}