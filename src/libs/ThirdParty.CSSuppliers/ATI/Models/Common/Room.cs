namespace ThirdParty.CSSuppliers.ATI.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Room
    {
        public string RoomTypeCode { get; set; } = string.Empty;

        [XmlArray("RoomRates")]
        [XmlArrayItem("RoomRate")]
        public RoomRate[] RoomRates { get; set; } = Array.Empty<RoomRate>();

        public int ID { get; set; }

        public int Adults { get; set; }

        public int Children { get; set; }

        public int Infants { get; set; }

        public string hlpChildAgeCSV { get; set; } = string.Empty;
    }
}
