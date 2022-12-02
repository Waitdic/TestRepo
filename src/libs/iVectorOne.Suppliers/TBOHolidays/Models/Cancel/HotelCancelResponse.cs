using iVectorOne.Suppliers.TBOHolidays.Models.Common;

namespace iVectorOne.Suppliers.TBOHolidays.Models.Cancel
{
    public class HotelCancelResponse
    {
        public Status Status { get; set; } = new();

        public RequestStatus RequestStatus { get; set; }
    }
}
