namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.List
{
    using iVectorOne_Admin_Api.Features.Shared;

    public record Response : ResponseBase
    {
        public void Default(TenantDto tenant)
        {
            Result = Results.Ok(tenant);
        }

        public void NotFound()
        {
            Result = Results.NotFound();
        }
    }
}