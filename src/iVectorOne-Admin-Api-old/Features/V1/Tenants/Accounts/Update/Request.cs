namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Update
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }
        public int AccountId { get; set; }
        public AccountDto Account { get; set; } = null!;
    }
}