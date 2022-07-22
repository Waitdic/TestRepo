namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.BookingItinerary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.Book;

    public class BookingItineraryResponse : IExpediaRapidResponse<BookingItineraryResponse>
    {
        [JsonProperty("itinerary_id")]
        public string ItineraryID { get; set; }

        [JsonProperty("property_id")]
        public string PropertyID { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, Link> Links { get; set; } = new Dictionary<string, Link>();

        [JsonProperty("rooms")]
        public List<BookingItineraryResponseRoom> Rooms { get; set; }

        [JsonProperty("billing_contact")]
        public BillingContact BillingContact { get; set; }

        [JsonProperty("adjustment")]
        public Rate Adjustment { get; set; }

        [JsonProperty("creation_date_time")]
        public DateTime CreationDateTime { get; set; }

        [JsonProperty("affiliate_reference_id")]
        public string AffiliateReferenceID { get; set; }

        [JsonProperty("affiliate_metadata")]
        public string AffiliateMetadata { get; set; }

        [JsonProperty("conversations")]
        public Conversations Conversations { get; set; }

        public (bool valid, BookingItineraryResponse response) GetValidResults(string responseString, int statusCode)
        {
            (bool valid, BookingItineraryResponse response) = (false, new BookingItineraryResponse());

            if (!string.IsNullOrWhiteSpace(responseString))
            {
                try
                {
                    response = JsonConvert.DeserializeObject<BookingItineraryResponse>(responseString)!;

                    if (response.Rooms.First() is not null && response.Rooms.First().Rate is not null)
                    {
                        valid = true;
                    }
                }
                catch
                {
                }
            }

            return (valid, response);
        }
    }
}