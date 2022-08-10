namespace iVectorOne_Admin_Api.Features.V1.Tenants.Disable
{
    public record ResponseModel : ResponseModelBase
    {
        public int TenantId { get; set; }
    }
}
