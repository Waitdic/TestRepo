namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.List
{
    using iVectorOne_Admin_Api.Config.Models;

    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.Login));
        }
    }
}