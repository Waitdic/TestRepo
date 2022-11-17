using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Suppliers.Info
{
    public record Request : IRequest<ResponseBase>
    {
        public int SupplierID { get; set; }
    }
}