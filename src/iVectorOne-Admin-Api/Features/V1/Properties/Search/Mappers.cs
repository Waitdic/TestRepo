using iVectorOne_Admin_Api.Data.Models;

namespace iVectorOne_Admin_Api.Features.V1.Properties.Search
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.PropertyId, act => act.MapFrom(src => src.CentralPropertyID))
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name));
        }
    }
}
