namespace ThirdParty.CSSuppliers.TBOHolidays.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class RoomCombination
    {
        [XmlElement("RoomIndex")]
        public int[] RoomIndex { get; set; } = Array.Empty<int>();
    }
}
