namespace iVectorOne.Suppliers.TBOHolidays.Models.Common
{
    using Newtonsoft.Json;

    public class Status
    {
        [JsonProperty("Code")]
        public int Code { get; set; }


        [JsonProperty("Description")]
        public string Description { get; set; } = string.Empty;
    }
}
