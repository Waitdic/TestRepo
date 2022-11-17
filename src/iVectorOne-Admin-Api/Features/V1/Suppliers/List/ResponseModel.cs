namespace iVectorOne_Admin_Api.Features.V1.Suppliers.List
{
    public record ResponseModel : ResponseModelBase
    {
        public List<SupplierDto> Suppliers { get; set; } = new();
    }
}