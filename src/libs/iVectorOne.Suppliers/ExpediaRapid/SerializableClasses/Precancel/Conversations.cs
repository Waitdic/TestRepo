namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.BookingItinerary
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Conversations
    {

        [JsonProperty("links")]
        public Dictionary<string, Link> Links { get; set; }

    }

}