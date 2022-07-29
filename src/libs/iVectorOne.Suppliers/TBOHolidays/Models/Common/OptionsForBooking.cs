namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class OptionsForBooking
    {
        public bool FixedFormat { get; set; }

        [XmlElement("RoomCombination")]
        public RoomCombination[] RoomCombination { get; set; } = Array.Empty<RoomCombination>();
    }
}
