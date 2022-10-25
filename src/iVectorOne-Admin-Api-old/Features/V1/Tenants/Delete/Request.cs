namespace iVectorOne_Admin_Api.Features.V1.Tenants.Delete
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }
    }
}
