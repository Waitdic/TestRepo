﻿namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Create
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<AccountDto, Account>()
                .ForMember(dest => dest.Login, act => act.MapFrom(src => src.UserName));
        }
    }
}