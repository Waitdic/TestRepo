using System.Text.Json.Serialization;

namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    public class TenantDto
    {
        public int TenantId { get; set; }

        [JsonPropertyName("name")]
        public string CompanyName { get; set; } = null!;

        public Guid TenantKey { get; set; }
    }
}
