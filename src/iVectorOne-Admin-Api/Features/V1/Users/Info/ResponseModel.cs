namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    using System.Text.Json.Serialization;

    public record ResponseModel : ResponseModelBase
    {
        [JsonPropertyName("fullName")]
        public string UserName { get; set; } = string.Empty;

        public int UserId { get; set; }

        public List<TenantDto> Tenants { get; set; } = new();

        public List<AuthorisationDto> Authorisations { get; set; } = new();
    }
}