namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class SearchResponseRoom
    {

        [JsonProperty("id")]
        public string RoomID { get; set; }

        [JsonProperty("room_name")]
        public string RoomName { get; set; }

        [JsonProperty("rates")]
        public List<RoomRate> Rates { get; set; } = new List<RoomRate>();

    }

}