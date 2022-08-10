using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.Delete
{
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
