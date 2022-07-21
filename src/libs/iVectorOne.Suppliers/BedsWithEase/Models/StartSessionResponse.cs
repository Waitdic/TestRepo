namespace iVectorOne.Suppliers.BedsWithEase.Models
{
    using Common;

    public class StartSessionResponse : SoapContent
    {
        public string SessionId { get; set; } = string.Empty;
    }
}