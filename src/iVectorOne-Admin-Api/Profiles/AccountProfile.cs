namespace iVectorOne_Admin_Api.Profiles
{
    using iVectorOne_Admin_Api.Config.Models;

    public class AccountProfile : Profile
    {
        public AccountProfile()
        {

            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.UserName, act=> act.MapFrom(src=> src.Login));
            CreateMap<AccountSupplier, SupplierDTO>()
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Supplier.SupplierName));
            CreateMap<Supplier, SupplierListItemDTO>()
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.SupplierName));
        }
    }
}