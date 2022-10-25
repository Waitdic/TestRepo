namespace iVectorOne_Admin_Api.Features.V1.Tenants.Info
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<Tenant, TenantDto>();
        }
    }
}
