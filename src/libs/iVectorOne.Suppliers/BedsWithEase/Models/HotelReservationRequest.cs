namespace iVectorOne.CSSuppliers.BedsWithEase.Models
{
    using Common;

    public class HotelReservationRequest : SoapContent
    {
        public string SessionId { get; set; } = string.Empty;

        public string BookCode { get; set; } = string.Empty;
    }
}
