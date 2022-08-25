namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.List
{
    public record AccountDto
    {
        public int AccountId { get; set; }

        public List<SupplierDto> AccountSuppliers { get; set; } = new();
    }
}