namespace iVectorOne_Admin_Api.Config.Requests
{
    using iVectorOne_Admin_Api.Config.Responses;

    public class SupplierRequest : IRequest<SupplierResponse>
    {
        public SupplierRequest(int tenantId)
        {
            TenantId = tenantId;
        }

        public int TenantId { get; set; }
        public int SupplierId { get; set; }
        public int AccountId { get; set; }
    }
}