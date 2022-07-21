namespace iVectorOne.Suppliers.OceanBeds.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("ConfirmCancellationRS", Namespace = "http://oceanbeds.com/2014/10")]
    public class ConfirmCancellationRS
    {
        [XmlArray("Response")]
        [XmlArrayItem("CancellationBookingList")]
        public CancellationBookingList[] Response { get; set; } = Array.Empty<CancellationBookingList>();

        public Status Status { get; set; } = new();
    }
}
