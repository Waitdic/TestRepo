using iVectorOne.Suppliers.TBOHolidays.Models.Common;

namespace iVectorOne.Suppliers.TBOHolidays.Models
{
    public class HotelCancelResponse : SoapContent
    {
        public Status Status { get; set; } = new();

        public RequestStatus RequestStatus { get; set; }
    }
}
