using AutoMapper;
using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Profiles
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<Subscription, SubscriptionDTO>()
                .ForMember(dest => dest.UserName, act=> act.MapFrom(src=> src.Login));
        }
    }
}