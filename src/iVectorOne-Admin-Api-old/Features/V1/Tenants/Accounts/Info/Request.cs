namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Info
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public int AccountId { get; set; }
    }
}