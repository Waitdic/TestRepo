using System.Text.Json.Serialization;

namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    public record ResponseModel : ResponseModelBase
    {
        [JsonPropertyName("fullName")]
        public string UserName { get; set; } = string.Empty;

        public List<TenantDto> Tenants { get; set; } = new List<TenantDto>();

        public List<AuthorisationDto> Authorisations { get; set; } = new List<AuthorisationDto>();
    }
}
