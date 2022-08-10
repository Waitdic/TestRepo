using iVectorOne_Admin_Api.Features.Shared;
//using iVectorOne_Admin_Api.Features.V1.Users.List;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Info
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
