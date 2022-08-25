namespace iVectorOne_Admin_Api.Config.Requests
{
    using iVectorOne_Admin_Api.Config.Responses;

    public class AccountSupplierRequest : IRequest<AccountSupplierResponse>
    {
        public AccountSupplierRequest(int tenantId)
        {
            TenantId = tenantId;
        }

        public int TenantId { get; set; }

        public int AccountId { get; set; }
    }
}