namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Delete
{
    using iVectorOne_Admin_Api.Features.Shared;

    public record Response : ResponseBase
    {
        public void Default()
        {
            Result = Results.Ok();
        }

        public void NotFound()
        {
            Result = Results.NotFound();
        }
    }
}