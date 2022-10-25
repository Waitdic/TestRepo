namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Link
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public int UserId { get; set; }

        public string Relationship { get; set; } = "";
    }
}
