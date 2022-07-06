using ThirdParty.CSSuppliers.TBOHolidays.Models.Common;

namespace ThirdParty.CSSuppliers.TBOHolidays.Models
{
    public class HotelCancelResponse : SoapContent
    {
        public Status Status { get; set; } = new();

        public RequestStatus RequestStatus { get; set; }
    }
}
