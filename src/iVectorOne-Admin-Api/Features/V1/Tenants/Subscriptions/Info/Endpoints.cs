using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Info
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantSubscriptionInfoV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/tenants/{tenantid}/subscriptions/{subscriptionid}", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantId, int subscriptionId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId, SubscriptionId = subscriptionId });

                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}