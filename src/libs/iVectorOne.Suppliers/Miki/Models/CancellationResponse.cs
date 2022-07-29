namespace iVectorOne.Suppliers.Miki.Models
{
    using System;
    using System.Xml.Serialization;
    using Common;

    public class CancellationResponse : SoapContent
    {
        [XmlArray("cancelledTours")]
        [XmlArrayItem("cancelledTour")]
        public CancelledTour[] CancelledTours { get; set; } = Array.Empty<CancelledTour>();
    }
}
