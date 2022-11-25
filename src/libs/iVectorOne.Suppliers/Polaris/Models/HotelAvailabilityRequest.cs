namespace iVectorOne.Suppliers.Polaris.Models
{
    using System.Text.Json.Serialization;


    public class HotelAvailabilityRequest
    {
        [JsonPropertyName("searchAvail")]
        public SearchAvail SearchAvail { get; set; } = new();

        [JsonPropertyName("timeout")]
        public int Timeout { get; set; }
    }

    public class HotelCategory
    {

    }

    public class RoomRate
    {

    }

    public class RoomInfo 
    {
    
    }

}
