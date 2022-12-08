using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Info
{
    public record Request : IRequest<ResponseBase>
    {
        public int TenantID { get; set; }

        public int AccountID { get; set; }


        public int SupplierID { get; set; }
    }
}