namespace iVectorOne_Admin_Api.Features.V1.Tenants.Info
{
    public record ResponseModel : ResponseModelBase
    {
        public TenantDto Tenant { get; set; } = new TenantDto();
    }
}
