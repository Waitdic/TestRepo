namespace iVectorOne_Admin_Api.Data
{
    public partial class Account
    {
        public int AccountId { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool? DummyResponses { get; set; }
        public short PropertyTpRequestLimit { get; set; }
        public short SearchTimeoutSeconds { get; set; }
        public bool LogMainSearchError { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public string Environment { get; set; } = null!;
        public int TenantId { get; set; }
        public string Status { get; set; } = null!;
        public string? EncryptedPassword { get; set; } = null!;

        public Tenant? Tenant { get; set; }
        public List<AccountSupplierAttribute> AccountSupplierAttributes { get; set; } = new();
        public List<AccountSupplier> AccountSuppliers { get; set; } = new();
    }
}