namespace iVectorOne_Admin_Api.Config.Requests
{
    using iVectorOne_Admin_Api.Config.Responses;

    public class SupplierAttributeRequest : IRequest<SupplierAttributeResponse>
    {
        public int SupplierID { get; set; }
    }
}