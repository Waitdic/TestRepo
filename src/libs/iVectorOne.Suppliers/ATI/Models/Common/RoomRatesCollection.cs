namespace ThirdParty.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot("RoomRatesCollection")]
    public class RoomRatesCollection
    {
        [XmlElement("RoomRates")]
        public RoomRates[] RoomRates { get; set; } = Array.Empty<RoomRates>();
    }
}
