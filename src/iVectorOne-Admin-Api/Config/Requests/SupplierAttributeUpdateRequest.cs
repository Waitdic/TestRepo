using iVectorOne_Admin_Api.Config.Models;
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
        public List<SupplierSubscriptionAttributeItem> Attributes { get; set; } = new List<SupplierSubscriptionAttributeItem>();
    }
}