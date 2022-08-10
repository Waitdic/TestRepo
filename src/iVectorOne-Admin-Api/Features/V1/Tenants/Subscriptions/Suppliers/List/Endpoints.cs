using iVectorOne_Admin_Api.Data;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.List
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantSubscriptionSupplierListV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/tenants/{tenantid}/subscriptions/{subscriptionid}/suppliers", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantId, int subscriptionId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId, SubscriptionId = subscriptionId });

                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}