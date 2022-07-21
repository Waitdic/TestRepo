using System;

namespace iVectorOne.Suppliers.Models.W2M
{
#pragma warning disable CS8618

    public class PreBookRequestParameters
    {
        public PreBookRequestParameters(BaseRequestParameters baseRequestParameters, DateTime arrivalDate, DateTime departureDate,
            int duration, string hotelCode, string source, string leadGuestNationality)
        {
            BaseRequestParameters = baseRequestParameters;
            ArrivalDate = arrivalDate;
            DepartureDate = departureDate;
            Duration = duration;
            HotelCode = hotelCode;
            Source = source;
            LeadGuestNationality = leadGuestNationality;
        }

        public BaseRequestParameters BaseRequestParameters { get; }
        public DateTime ArrivalDate { get; }
        public DateTime DepartureDate { get; }
        public string HotelCode { get; }
        public int Duration { get; }
        public string Source { get; }
        public string LeadGuestNationality { get; }
    }
}
