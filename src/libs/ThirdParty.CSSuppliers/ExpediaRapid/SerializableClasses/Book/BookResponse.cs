namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Book
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class BookResponse : IExpediaRapidResponse
    {

        [JsonProperty("itinerary_id")]
        public string ItineraryID { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, Link> Links { get; set; } = new Dictionary<string, Link>();

        public bool IsValid(string responseString, int statusCode)
        {

            if (!string.IsNullOrWhiteSpace(responseString))
            {
                try
                {
                    var bookResponse = JsonConvert.DeserializeObject<BookResponse>(responseString);

                    if (string.IsNullOrWhiteSpace(bookResponse.ItineraryID) || !bookResponse.Links.ContainsKey("retrieve"))
                        return false;

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

    }

}