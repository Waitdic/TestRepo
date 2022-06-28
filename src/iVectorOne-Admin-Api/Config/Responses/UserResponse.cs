using iVectorOne_Admin_Api.Config.Models;
using System.Text.Json.Serialization;

namespace iVectorOne_Admin_Api.Config.Responses
{
    public class UserResponse
    {
        public bool Success { get; set; }
        [JsonPropertyName("fullName")]
        public string UserName { get; set; } = string.Empty;
        public List<TenantDTO> Tenants { get; set; } = new List<TenantDTO>();
        public List<string> Warnings { get; set; } = new List<string>();
    }
}