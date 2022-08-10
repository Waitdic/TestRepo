using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.List
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.Login));
        }
    }
}
