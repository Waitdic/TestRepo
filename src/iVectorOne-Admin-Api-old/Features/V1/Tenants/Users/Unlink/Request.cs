namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Unlink
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public int UserId { get; set; }
    }
}
