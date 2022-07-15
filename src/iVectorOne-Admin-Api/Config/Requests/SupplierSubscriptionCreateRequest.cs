using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Config.Responses;
using MediatR;

namespace iVectorOne_Admin_Api.Config.Requests
{
    public class SupplierSubscriptionCreateRequest : IRequest<SupplierSubscriptionCreateResponse>
    {
        public SupplierSubscriptionCreateRequest(int tenantId)
        {
            TenantId = tenantId;
        }

        public int TenantId { get; set; }

        public int SubscriptionId { get; set; }

        public int SupplierId { get; set; }

        public List<SupplierAttributeItem> SupplierAttributes { get; set; } = new List<SupplierAttributeItem>();
    }
}
