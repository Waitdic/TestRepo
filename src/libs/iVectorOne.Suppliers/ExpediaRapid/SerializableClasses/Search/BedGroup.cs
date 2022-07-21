namespace iVectorOne.CSSuppliers.ExpediaRapid.SerializableClasses.Search
{
    using System.Collections.Generic;
    using Newtonsoft.Json;


    public class BedGroupAvailability
    {

        [JsonProperty("links")]
        public BedGroupAvailabilityLink Links { get; set; }

        [JsonProperty("id")]
        public string BedGroupID { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("configuration")]
        public List<BedConfiguration> BedConfigurations { get; set; } = new List<BedConfiguration>();

    }

}