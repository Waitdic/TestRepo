namespace ThirdParty.CSSuppliers.BedsWithEase.Models
{
    using Common;

    public class StartSessionRequest : SoapContent
    {
        public string UserId { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string LanguageCode { get; set; } = string.Empty;
    }
}
