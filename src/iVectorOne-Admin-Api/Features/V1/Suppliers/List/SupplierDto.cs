namespace iVectorOne_Admin_Api.Features.V1.Suppliers.List
{
    public record SupplierDto
    {
        public int SupplierID { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}