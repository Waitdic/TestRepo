namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Unlink
{
    public record ResponseModel : ResponseModelBase
    {
        public int TenantId { get; set; }
    }
}
