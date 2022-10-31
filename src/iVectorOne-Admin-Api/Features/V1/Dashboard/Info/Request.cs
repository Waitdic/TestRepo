using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public record Request : IRequest<ResponseBase>
    {
        public int TenantId { get; set; }

        public int AccountId { get; set; }
    }
}
