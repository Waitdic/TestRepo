namespace iVectorOne_Admin_Api.Config.Requests
{
    using iVectorOne_Admin_Api.Config.Models;
    using iVectorOne_Admin_Api.Config.Responses;
    using MediatR;

    public class SupplierAttributeUpdateRequest : IRequest<SupplierAttributeUpdateResponse>
    {
        public SupplierAttributeUpdateRequest(int tenantid)
        {
            TenantId = tenantid;
        }

        public int TenantId { get; set; }
        public int AccountId { get; set; }
        public int SupplierId { get; set; }
        public List<AccountSupplierAttributeItem> Attributes { get; set; } = new();
    }
}