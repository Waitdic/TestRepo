namespace ThirdParty.CSSuppliers.AmadeusHotels.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class BasicPropertyInfo
    {
        [XmlArray("VendorMessages")]
        [XmlArrayItem("VendorMessage")]
        public VendorMessage[] VendorMessages { get; set; } = Array.Empty<VendorMessage>();

        [XmlAttribute]
        public string ChainCode { get; set; } = string.Empty;

        [XmlAttribute]
        public string HotelCode { get; set; } = string.Empty;
    }
}
