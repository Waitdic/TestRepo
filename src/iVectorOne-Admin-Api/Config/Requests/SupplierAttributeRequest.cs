using iVectorOne_Admin_Api.Config.Responses;
using MediatR;

namespace iVectorOne_Admin_Api.Config.Requests
{
    public class SupplierAttributeRequest : IRequest<SupplierAttributeResponse>
    {
        public int SupplierID { get; set; }
    }
}
