namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.List
{
    public record SupplierDto
    {
        public int AccountSupplierID { get; set; }

        public int SupplierID { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Enabled { get; set; } = false;
    }
}