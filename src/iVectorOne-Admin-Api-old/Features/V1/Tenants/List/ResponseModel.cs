namespace iVectorOne_Admin_Api.Features.V1.Tenants.List
{
    public record ResponseModel : ResponseModelBase
    {
        public List<TenantDto> Tenants { get; set; } = new List<TenantDto>();
    }
}
