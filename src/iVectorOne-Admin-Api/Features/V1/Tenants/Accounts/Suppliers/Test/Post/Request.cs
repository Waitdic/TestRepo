using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test.Post
{
    public class Request : IRequest<ResponseBase>
    {
        public int AccountID { get; set; }

        public int SupplierID { get; set; }
    }
}
