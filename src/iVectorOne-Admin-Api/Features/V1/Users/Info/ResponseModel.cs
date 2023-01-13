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

    #region DTO

    public class TenantDto
    {
        public string UserKey { get; set; } = "";

        public int TenantId { get; set; }

        [JsonPropertyName("name")]
        public string CompanyName { get; set; } = null!;

        public Guid TenantKey { get; set; }
    }

    public class AuthorisationDto
    {
        public string User { get; set; } = string.Empty;

        public string Relationship { get; set; } = string.Empty;

        public string Object { get; set; } = string.Empty;
    }

    #endregion
}