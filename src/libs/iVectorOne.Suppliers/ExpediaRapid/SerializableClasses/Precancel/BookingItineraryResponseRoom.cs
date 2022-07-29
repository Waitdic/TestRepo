namespace iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.BookingItinerary
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using iVectorOne.Suppliers.ExpediaRapid.SerializableClasses.Book;

    public class BookingItineraryResponseRoom : BookRequestRoom
    {

        [JsonProperty("id")]
        public string RoomID { get; set; }

        [JsonProperty("confirmation_id")]
        public ConfirmationID ConfirmationID { get; set; }

        [JsonProperty("bed_group_id")]
        public string BedGroupID { get; set; }

        [JsonProperty("checkin")]
        public DateTime CheckIn { get; set; }

        [JsonProperty("checkout")]
        public DateTime CheckOut { get; set; }

        [JsonProperty("number_of_adults")]
        public int NumberOfAdults { get; set; }

        [JsonProperty("child_ages")]
        public List<int> ChildAges { get; set; } = new List<int>();

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("rate")]
        public BookingItineraryResponseRoomRate Rate { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, Link> Links { get; set; } = new Dictionary<string, Link>();

    }

}