namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Info
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.Login))
                .ForMember(dest => dest.Password, act => act.MapFrom(src => src.EncryptedPassword));
        }
    }
}