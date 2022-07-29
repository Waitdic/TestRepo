namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class HotelCombinations
    {
        [XmlElement("HotelCombination")]
        public HotelCombination[] HotelCombination { get; set; } = Array.Empty<HotelCombination>();
    }
}
