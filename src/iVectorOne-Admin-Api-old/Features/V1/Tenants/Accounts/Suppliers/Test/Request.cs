namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    public class Request : IRequest<Response>
    {
        public int AccountID { get; set; }

        public int SupplierID { get; set; }
    }
}
