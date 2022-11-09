using iVectorOne_Admin_Api.Data.Models;
using iVectorOne_Admin_Api.Features.V1.Properties.Search;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.BookingViewer
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<BookingLog, LogEntry>()
                .ForMember(dest => dest.Timestamp, act => act.MapFrom(src => src.BookingDateTime))
                .ForMember(dest => dest.Succesful, act => act.MapFrom(src => src.Success));
        }
    }
}
