using iVectorOne_Admin_Api.Data.Models;
using iVectorOne_Admin_Api.Features.V1.Properties.Search;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewer
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<iVectorOne_Admin_Api.Data.Models.LogEntry, LogEntry>()
                .ForMember(dest => dest.Timestamp, act => act.MapFrom(src => src.RequestDateTime))
                .ForMember(dest => dest.Succesful, act => act.MapFrom(src => src.Successful));
                }
    }
}