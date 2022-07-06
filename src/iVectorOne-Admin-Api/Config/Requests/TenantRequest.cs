using iVectorOne_Admin_Api.Config.Responses;
using MediatR;

namespace iVectorOne_Admin_Api.Config.Requests
{
    public class TenantRequest : IRequest<TenantResponse>
    {
        public TenantRequest(int tenantId)
        {
            TenantId = tenantId;
        }

        public int TenantId { get; set; }
    }
}
