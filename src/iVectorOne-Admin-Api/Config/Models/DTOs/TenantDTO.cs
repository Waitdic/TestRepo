namespace iVectorOne_Admin_Api.Config.Models
{
    using System.Text.Json.Serialization;

    public class TenantDTO
    {
        public int TenantId { get; set; }
        [JsonPropertyName("name")]
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? ContactTelephone { get; set; }
        public string? ContactEmail { get; set; }

        [JsonIgnore]
        public string Status { get; set; } = null!;

        public bool IsActive => Status.ToLower() == RecordStatus.Active;
        public Guid TenantKey { get; set; }
    }
}