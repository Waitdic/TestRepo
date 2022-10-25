namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    using iVectorOne_Admin_Api.Features.Shared;

    public record Response : ResponseBase
    {
        public void Default(SupplierTestResponse response)
        {
            Result = Results.Ok(response);
        }

        public void NotFound()
        {
            Result = Results.NotFound();
        }
    }
}
