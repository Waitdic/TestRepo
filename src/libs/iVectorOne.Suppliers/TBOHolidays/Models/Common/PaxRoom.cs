namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class PaxRoom
    {
        [JsonProperty("Adults")]
        public int Adults { get; set; }

        [JsonProperty("Children")]
        public int Children { get; set; }

        [JsonProperty("ChildrenAges")]
        public List<int> ChildrenAges { get; set; } = new();
    }
}