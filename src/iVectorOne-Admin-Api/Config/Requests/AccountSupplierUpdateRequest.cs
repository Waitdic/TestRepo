namespace iVectorOne_Admin_Api.Config.Requests
{
    using iVectorOne_Admin_Api.Config.Responses;

    public class AccountSupplierUpdateRequest : IRequest<AccountSupplierUpdateResponse>
    {
        public AccountSupplierUpdateRequest(int tenantId)
        {
            TenantId = tenantId;
        }

        public int TenantId { get; set; }
        public int AccountId { get; set; }
        public int SupplierId { get; set; }
        public bool Enabled { get; set; }
    }
}