namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.List
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<SupplierSubscription, SupplierDto>()
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Supplier.SupplierName));
        }
    }
}
