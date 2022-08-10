namespace iVectorOne.Suppliers.GoGlobal.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    public class SearchRq : Main
    {
        public string MaximumWaitTime { get; set; } = string.Empty;
        public bool ShouldSerializeMaximumWaitTime() => !string.IsNullOrEmpty(MaximumWaitTime);

        public string CityCode { get; set; } = string.Empty;
        public bool ShouldSerializeCityCode() => !string.IsNullOrEmpty(CityCode);

        [XmlArray("Hotels")]
        [XmlArrayItem("HotelId")]
        public List<string> Hotels { get; set; } = new();
        public bool ShouldSerializeHotels() => Hotels.Any();

        public string ArrivalDate { get; set; } = string.Empty;
        public string Nights { get; set; } = string.Empty;
        public List<Room> Rooms { get; set; } = new();

        public string Nationality { get; set; } = string.Empty;
        public bool ShouldSerializeNationality() => !string.IsNullOrEmpty(Nationality);
    }
}
