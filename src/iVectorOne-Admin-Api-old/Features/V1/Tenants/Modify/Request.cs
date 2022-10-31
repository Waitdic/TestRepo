namespace iVectorOne_Admin_Api.Features.V1.Tenants.Modify
{
    public record Request : IRequest<Response>
    {
        public int TenantId { get; set; }

        public string ContactName { get; set; } = "";

        public string ContactTelephone { get; set; } = "";

        public string ContactEmail { get; set; } = "";
    }
}
