using iVectorOne_Admin_Api.Data.Models;
using iVectorOne_Admin_Api.Features.V1.Properties.Search;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.BookingViewer
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<BookingLog, LogEntry>()
                .ForMember(dest => dest.Timestamp, act => act.MapFrom(src => src.BookingDateTime));

                //.ForMember(dest => dest.PropertyId, act => act.MapFrom(src => src.CentralPropertyID))
                //.ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name));

            //            SELECT T1.BookingID,
            //T1.BookingDateTime,
            //T4.SupplierName,
            //T2.[Type],
            //T2.Success,
            //T3.ResponseTime,
            //T1.SupplierBookingReference,
            //T1.LeadGuestName,
            //T2.RequestLog,
            //T2.ResponseLog,
            //T3.RequestLog SupplierRequestLog,
                }
    }
}
