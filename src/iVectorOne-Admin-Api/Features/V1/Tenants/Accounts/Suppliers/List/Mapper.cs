namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.List
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<AccountSupplier, SupplierDto>()
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Supplier.SupplierName));
        }
    }
}