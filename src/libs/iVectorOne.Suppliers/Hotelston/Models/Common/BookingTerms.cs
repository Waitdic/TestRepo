namespace iVectorOne.CSSuppliers.Hotelston.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class BookingTerms
    {
        [XmlAttribute("nonRefundable")]
        public bool NonRefundable { get; set; }

        [XmlElement("cancellationPolicy")]
        public CancellationPolicy[] CancellationPolicies { get; set; } = Array.Empty<CancellationPolicy>();

        [XmlElement("bookingRemarks")]
        public string BookingRemarks { get; set; } = string.Empty;
    }
}