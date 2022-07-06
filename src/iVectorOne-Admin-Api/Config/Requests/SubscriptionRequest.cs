using iVectorOne_Admin_Api.Config.Responses;
using MediatR;

namespace iVectorOne_Admin_Api.Config.Requests
{
    public class SubscriptionRequest : IRequest<SubscriptionResponse>
    {
        public SubscriptionRequest(int tenantId)
        {
            TenantId = tenantId;
        }

        public int TenantId { get; set; }

        public int SubscriptionId { get; set; }
    }
}
