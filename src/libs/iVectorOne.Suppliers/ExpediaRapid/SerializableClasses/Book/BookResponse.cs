namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.Book
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class BookResponse : IExpediaRapidResponse<BookResponse>
    {

        [JsonProperty("itinerary_id")]
        public string ItineraryID { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, Link> Links { get; set; } = new Dictionary<string, Link>();

        public (bool valid, BookResponse response) GetValidResults(string responseString, int statusCode)
        {
            (bool valid, BookResponse bookResponse) = (false, new BookResponse());

            if (!string.IsNullOrWhiteSpace(responseString))
            {
                try
                {
                    bookResponse = JsonConvert.DeserializeObject<BookResponse>(responseString)!;

                    if (!string.IsNullOrWhiteSpace(bookResponse.ItineraryID) && bookResponse.Links.ContainsKey("retrieve"))
                    {
                        valid = true;
                    }
                }
                catch
                {
                }
            }

            return (valid, bookResponse);
        }
    }
}