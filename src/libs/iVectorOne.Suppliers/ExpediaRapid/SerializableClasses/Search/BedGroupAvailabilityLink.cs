namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using Newtonsoft.Json;

    public class BedGroupAvailabilityLink
    {

        [JsonProperty("price_check")]
        public Link PriceCheckLink { get; set; }

    }
}