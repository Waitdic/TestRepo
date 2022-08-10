using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.List
{
    public record Response : ResponseBase
    {
        public void Default(SubscriptionDto subscription)
        {
            Result = Results.Ok(subscription);
        }

        public void NotFound()
        {
            Result = Results.NotFound();
        }
    }
}
