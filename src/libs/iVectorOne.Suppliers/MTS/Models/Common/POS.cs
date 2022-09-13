namespace iVectorOne.Suppliers.MTS.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class POS
    {
        [XmlElement("Source")]
        public Source[] Source { get; set; } = Array.Empty<Source>();
    }

    public class Source
    {
        public RequestorID RequestorID { get; set; } = new();

        public BookingChannel? BookingChannel { get; set; }
        public bool ShouldSerializeBookingChannel() => BookingChannel != null;
    }

    public class RequestorID
    {
        [XmlAttribute]
        public string Instance { get; set; } = string.Empty;
        public bool ShouldSerializeInstance() => !string.IsNullOrEmpty(Instance);

        [XmlAttribute]
        public string ID_Context { get; set; } = string.Empty;
        public bool ShouldSerializeID_Context() => !string.IsNullOrEmpty(ID_Context);

        [XmlAttribute]
        public string ID { get; set; } = string.Empty;

        [XmlAttribute]
        public int Type { get; set; }

        [XmlAttribute]
        public string MessagePassword { get; set; } = string.Empty;
        public bool ShouldSerializeMessagePassword() => !string.IsNullOrEmpty(MessagePassword);
    }

    public class BookingChannel
    {
        [XmlAttribute]
        public int Type { get; set; }
    }
}
