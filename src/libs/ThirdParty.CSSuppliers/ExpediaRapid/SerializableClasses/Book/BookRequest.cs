namespace ThirdParty.CSSuppliers.ExpediaRapid.SerializableClasses.Book
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class BookRequest
    {

        [JsonProperty("affiliate_reference_id")]
        public string AffiliateReferenceId { get; set; }

        [JsonProperty("hold")]
        public bool Hold { get; set; }

        [JsonProperty("rooms")]
        public List<BookRequestRoom> Rooms { get; set; } = new List<BookRequestRoom>();

        [JsonProperty("payments")]
        public List<Payment> Payments { get; set; } = new List<Payment>();

        [JsonProperty("affiliate_metadata")]
        public string AffiliateMetadata { get; set; }

    }

}