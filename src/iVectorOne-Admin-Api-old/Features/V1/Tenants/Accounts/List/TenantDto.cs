namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.List
{
    public record TenantDto
    {
        public string TenantName { get; set; } = "";

        public int TenantId { get; set; }

        public List<AccountDto> Accounts { get; set; } = new();
    }
}