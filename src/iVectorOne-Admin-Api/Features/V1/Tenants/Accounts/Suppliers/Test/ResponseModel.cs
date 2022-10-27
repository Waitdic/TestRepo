namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    public record ResponseModel: ResponseModelBase
    {
        public Guid RequestKey { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
