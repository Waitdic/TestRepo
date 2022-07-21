namespace iVectorOne.Suppliers.OceanBeds.Models
{
    using System.Xml.Serialization;
    using Common;

    [XmlRoot("PropertyAvailabilityRQ", Namespace = "http://oceanbeds.com/2014/10")]
    public class PropertyAvailabilityRQ
    {
        public Credential? Credential { get; set; }

        public string CheckInDate { get; set; } = string.Empty;

        public string CheckOutDate { get; set; } = string.Empty;

        [XmlArray("RoomList")]
        [XmlArrayItem("RequestRoom")]
        public RequestRoom[]? RoomList { get; set; }

        public string PropertyCode { get; set; } = string.Empty;

        public int StateId { get; set; }

        public int CityId { get; set; }

        public string Region { get; set; } = string.Empty;

        public int CommunityId { get; set; }

        public int BedRoom { get; set; }

        public int Bathroom { get; set; }

        public string HomeType { get; set; } = string.Empty;

        public string IsAvailable { get; set; } = string.Empty;

        public string HotelSearch { get; set; } = string.Empty;

        public string GenericSearch { get; set; } = string.Empty;

        public string VillaSearch { get; set; } = string.Empty;

        public Filters Filters { get; set; } = new();
    }
}
