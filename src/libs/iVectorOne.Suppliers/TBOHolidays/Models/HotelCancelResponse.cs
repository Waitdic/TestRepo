using iVectorOne.CSSuppliers.TBOHolidays.Models.Common;

namespace iVectorOne.CSSuppliers.TBOHolidays.Models
{
    public class HotelCancelResponse : SoapContent
    {
        public Status Status { get; set; } = new();

        public RequestStatus RequestStatus { get; set; }
    }
}
