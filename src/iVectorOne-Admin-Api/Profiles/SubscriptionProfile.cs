﻿using AutoMapper;
using iVectorOne_Admin_Api.Config.Models;

namespace iVectorOne_Admin_Api.Profiles
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<Subscription, SubscriptionDTO>()
                .ForMember(dest => dest.UserName, act=> act.MapFrom(src=> src.Login));
            CreateMap<SupplierSubscription, SupplierDTO>()
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Supplier.SupplierName));
            CreateMap<Subscription, SupplierSubscriptionDTO>()
                .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.Login))
                .ForMember(dest=> dest.SupplierSubscriptions, act=> act.MapFrom(o=> o.SupplierSubscriptions));
        }
    }
}