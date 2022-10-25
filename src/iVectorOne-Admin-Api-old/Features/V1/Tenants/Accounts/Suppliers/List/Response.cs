namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.List
{
    using iVectorOne_Admin_Api.Features.Shared;

    public record Response : ResponseBase
    {
        public void Default(AccountDto account)
        {
            Result = Results.Ok(account);
        }

        public void NotFound()
        {
            Result = Results.NotFound();
        }
    }
}