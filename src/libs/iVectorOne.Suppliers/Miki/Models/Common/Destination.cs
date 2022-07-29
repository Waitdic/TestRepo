namespace iVectorOne.Suppliers.Miki.Models.Common
{
    using System;
    using System.Xml.Serialization;

    public class Destination
    {
        [XmlElement("hotelRefs")]
        public HotelRefs HotelRefs { get; set; } = new();

        [XmlArray("cityNumbers")]
        [XmlArrayItem("cityNumber")]
        public int[] CityNumbers { get; set; } = Array.Empty<int>();
        public bool ShouldSerializeCityNumbers() => CityNumbers != Array.Empty<int>();
    }
}
