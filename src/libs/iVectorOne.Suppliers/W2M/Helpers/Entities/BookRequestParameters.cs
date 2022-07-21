using System.Collections.Generic;

namespace ThirdParty.CSSuppliers.Models.W2M
{
#pragma warning disable CS8618

    public class BookRequestParameters
    {
        public BookRequestParameters(BaseRequestParameters baseParameters, string externalReference,
            string startDate, string endDate, string hotelRequest, string hotelCode, string currency,
            List<string> bookingCodes, LeadGuest leadGuest)
        {
            BaseParameters = baseParameters;
            ExternalReference = externalReference;
            StartDate = startDate;
            EndDate = endDate;
            HotelRequest = hotelRequest;
            HotelCode = hotelCode;
            Currency = currency;
            BookingCodes = bookingCodes;
            LeadGuest = leadGuest;
        }

        public BaseRequestParameters BaseParameters { get; }
        public string ExternalReference { get; }
        public string StartDate { get; }
        public string EndDate { get; }
        public string HotelRequest { get; }
        public string HotelCode { get; }
        public string Currency { get; }
        public List<string> BookingCodes { get; }
        public LeadGuest LeadGuest { get; }
    }
}
