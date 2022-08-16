using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Update
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantSubscriptionUpdateV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPut("v1/tenants/{tenantid}/subscriptions/{subscriptionid}", async (IMediator mediator,
                HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantId,
                int subscriptionId,
                [FromBody] SubscriptionDto subscription) =>
            {
                var response = await mediator.Send(new Request {TenantId = tenantId, SubscriptionId = subscriptionId, Subscription = subscription});

                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}