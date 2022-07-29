namespace iVectorOne.Suppliers.Hotelston.Models.Common
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Criteria
    {
        [XmlElement("checkIn")]
        public string CheckIn { get; set; } = string.Empty;

        [XmlElement("checkOut")]
        public string CheckOut { get; set; } = string.Empty;

        [XmlElement("cityId")]
        public string CityId { get; set; } = string.Empty;

        public bool ShouldSerializeCityId() => !string.IsNullOrEmpty(CityId);

        [XmlElement("hotelId")]
        public string HotelId { get; set; } = string.Empty;

        public bool ShouldSerializeHotelId() => !string.IsNullOrEmpty(HotelId);

        [XmlElement("room")]
        public List<Room> Rooms { get; set; } = new();
    }
}