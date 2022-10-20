using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public record Request : IRequest<ResponseBase>
    {
        public int AccountID { get; set; }

        public int SupplierID { get; set; }

        public SearchRequest SearchRequest { get; set; }
    }
}
