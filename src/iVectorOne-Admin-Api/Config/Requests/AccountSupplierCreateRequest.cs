namespace iVectorOne_Admin_Api.Config.Requests
{
    using iVectorOne_Admin_Api.Config.Models;
    using iVectorOne_Admin_Api.Config.Responses;
    using MediatR;

    public class AccountSupplierCreateRequest : IRequest<AccountSupplierCreateResponse>
    {
        public AccountSupplierCreateRequest(int tenantId)
        {
            TenantId = tenantId;
        }

        public int TenantId { get; set; }

        public int AccountId { get; set; }

        public int SupplierId { get; set; }

        public List<SupplierAttributeItem> SupplierAttributes { get; set; } = new();
    }
}