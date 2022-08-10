namespace iVectorOne_Admin_Api.Features.V1.Tenants.Delete
{
    public record ResponseModel : ResponseModelBase
    {
        public int TenantId { get; set; }
    }
}
