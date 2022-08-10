namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Create
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<SubscriptionDto, Subscription>()
                .ForMember(dest => dest.Login, act => act.MapFrom(src => src.UserName));
        }
    }
}
