namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    using System.Text.Json.Serialization;

    public class TenantDto
    {
        public string UserKey { get; set; } = "";

        public int TenantId { get; set; }

        [JsonPropertyName("name")]
        public string CompanyName { get; set; } = null!;

        public Guid TenantKey { get; set; }
    }
}