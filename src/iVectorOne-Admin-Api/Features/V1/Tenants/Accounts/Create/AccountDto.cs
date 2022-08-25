namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Create
{
    public class AccountDto
    {
        public string UserName { get; set; } = string.Empty;

        public short PropertyTpRequestLimit { get; set; }

        public short SearchTimeoutSeconds { get; set; }

        public string CurrencyCode { get; set; } = string.Empty;
    }
}