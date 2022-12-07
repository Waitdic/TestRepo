namespace iVectorOne.Suppliers.Polaris.Models
{
    using Newtonsoft.Json;

    public class BookingReference
    {
        [JsonProperty("bookingReferenceID")]
        public string BookingReferenceId { get; set; } = string.Empty;
        public bool ShouldSerializeBookingReferenceId() => !string.IsNullOrEmpty(BookingReferenceId);

        [JsonProperty("requestReferenceID")]
        public string RequestReferenceId { get; set; } = string.Empty;
        public bool ShouldSerializeRequestReferenceId() => !string.IsNullOrEmpty(RequestReferenceId);
    }
}
