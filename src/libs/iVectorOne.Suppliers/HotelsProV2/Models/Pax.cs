namespace iVectorOne.Suppliers.HotelsProV2.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Pax
    {
        [JsonProperty("adult_quantity")]
        public int AdultQuantity { get; set; }

        [JsonProperty("children_ages")]
        public List<int> ChildAges { get; set; }
    }
}
