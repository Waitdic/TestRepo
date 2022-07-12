using iVectorOne_Admin_Api.Config.Responses;
using MediatR;

namespace iVectorOne_Admin_Api.Config.Requests
{
    public class SupplierSubscriptionUpdateRequest : IRequest<SupplierSubscriptionUpdateResponse>
    {
        public SupplierSubscriptionUpdateRequest(int tenantId)
        {
            TenantId = tenantId;
        }

        public int TenantId { get; set; }
        public int SupplierId { get; set; }
        public int SubscriptionId { get; set; }
        public bool Enabled { get; set; }
    }
}