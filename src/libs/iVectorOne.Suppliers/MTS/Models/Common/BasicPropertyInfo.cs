namespace iVectorOne.Suppliers.MTS.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class BasicPropertyInfo
    {
        [XmlAttribute]
        public string HotelCode { get; set; } = string.Empty;

        [XmlArray("VendorMessages")]
        [XmlArrayItem("VendorMessage")]
        public VendorMessage[] VendorMessages { get; set; } = Array.Empty<VendorMessage>();
        public bool ShouldSerializeVendorMessages() => VendorMessages.Length != 0;
    }

    public class VendorMessage
    {
        public string Title { get; set; } = string.Empty;

        public SubSection SubSection { get; set; } = new();
    }

    public class SubSection
    {
        public Paragraph Paragraph { get; set; } = new();
    }

    public class Paragraph
    {
        public string Text { get; set; } = string.Empty;
    }
}
