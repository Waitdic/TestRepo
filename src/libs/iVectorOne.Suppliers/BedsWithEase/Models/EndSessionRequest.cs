namespace iVectorOne.CSSuppliers.BedsWithEase.Models
{
    using Common;

    public class EndSessionRequest : SoapContent
    {
        public string SessionId { get; set; } = string.Empty;
    }
}
