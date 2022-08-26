namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Create
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }
        public AccountDto Account { get; set; } = null!;
    }
}