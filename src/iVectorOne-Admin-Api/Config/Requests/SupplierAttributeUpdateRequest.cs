using iVectorOne_Admin_Api.Config.Responses;
using MediatR;

namespace iVectorOne_Admin_Api.Config.Requests
{
    public class SupplierAttributeUpdateRequest : IRequest<SupplierAttributeUpdateResponse>
    {
        public SupplierAttributeUpdateRequest(int tenantid)
        {
            TenantId = tenantid;
        }

        public int TenantId { get; set; }
        public int SubscriptionId { get; set; }
        public int SupplierId { get; set; }
        public int SupplierSubscriptionAttributeId { get; set; }
        public string UpdatedValue { get; set; } = string.Empty;
    }
}