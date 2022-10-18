
using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Modify
{
    public record Request : IRequest<ResponseBase>
    {
        public int TenantId { get; set; }

        public string ContactName { get; set; } = "";

        public string ContactTelephone { get; set; } = "";

        public string ContactEmail { get; set; } = "";
    }
}
