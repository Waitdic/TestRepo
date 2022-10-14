namespace iVectorOne.Suppliers.Italcamel.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Booking
    {
        [XmlElement("STATUS")]
        public int Status { get; set; }
        public bool ShouldSerializeStatus() => Status != 0;

        [XmlElement("UID")]
        public string UID { get; set; } = string.Empty;

        [XmlElement("TYPE")]
        public string Type { get; set; } = string.Empty;

        [XmlElement("CHECKIN")]
        public string CheckIn { get; set; } = string.Empty;

        [XmlElement("CHECKOUT")]
        public string CheckOut { get; set; } = string.Empty;

        [XmlElement("ACCOMMODATIONUID")]
        public string AccomodationUID { get; set; } = string.Empty;
        public bool ShouldSerializeAccomodationUID() => !string.IsNullOrEmpty(AccomodationUID);

        [XmlElement("REQUESTEDPRICE")]
        public decimal RequestPrice { get; set; }

        [XmlElement("DELTAPRICE")]
        public decimal DeltaPrice { get; set; }

        [XmlArray("ROOMS")]
        [XmlArrayItem("ROOM")]
        public PrebookRoom[] Rooms { get; set; } = Array.Empty<PrebookRoom>();

        [XmlElement("NUMBER")]
        public string Number { get; set; } = string.Empty;

        [XmlElement("BOOKING")]
        public Booking? InternalBooking { get; set; }
        public bool ShouldSerializeInternalBooking() => InternalBooking != null;

        [XmlElement("REMARKS")]
        public string Remarks { get; set; } = string.Empty;
        public bool ShouldSerializeRemarks() => !string.IsNullOrEmpty(Remarks);

        [XmlArray("CANCELLATIONPOLICIES")]
        [XmlArrayItem("CANCELLATIONPOLICY")]
        public CancellationPolicy[] CancellationPolicies { get; set; } = Array.Empty<CancellationPolicy>();
        public bool ShouldSerializeCancellationPolicies() => CancellationPolicies.Length != 0;
    }
}
