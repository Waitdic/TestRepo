namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Update
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<AccountDto, Account>()
                .ForMember(dest => dest.Login, act => act.MapFrom(src => src.UserName))
                .ForMember(dest => dest.EncryptedPassword, act => act.MapFrom(src => src.Password))
                .ForMember(dest => dest.Password, opt => opt.Ignore());
        }
    }
}