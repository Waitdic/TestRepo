namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Delete
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public int AccountId { get; set; }
    }
}