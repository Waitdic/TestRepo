namespace iVectorOne.CSSuppliers.BedsWithEase.Models
{
    using Common;

    public class CancellationRequest : SoapContent
    {
        public string SessionId { get; set; } = string.Empty;

        public string OperatorCode { get; set; } = string.Empty;

        public string BookingReference { get; set; } = string.Empty;
    }
}
