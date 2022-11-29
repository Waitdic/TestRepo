namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using Newtonsoft.Json;

    public class Status
    {

        [JsonProperty("Code")]
        public string Code { get; set; } = string.Empty;


        [JsonProperty("Description")]
        public string Description { get; set; } = string.Empty;
    }
}
