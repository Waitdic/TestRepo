using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Create
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantSubscriptionCreateV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/{tenantid}/subscriptions", async (IMediator mediator,
                HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantId,
                [FromBody] SubscriptionDto subscription) =>
            {
                var response = await mediator.Send(new Request {TenantId = tenantId, Subscription = subscription });

                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}