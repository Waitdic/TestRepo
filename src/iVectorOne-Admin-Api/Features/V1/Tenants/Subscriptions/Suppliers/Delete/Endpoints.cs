using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Suppliers.Delete
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantSubscriptionSupplierDeleteV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapDelete("v1/tenants/{tenantid}/subscriptions/{subscriptionid}/suppliers/{supplierid}",
                async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey,
                    int tenantId, int subscriptionId, int supplierId) =>
                {
                    var response = await mediator.Send(new Request
                        {TenantId = tenantId, SubscriptionId = subscriptionId, SupplierId = supplierId});

                    return response.Result;

                }).RequireAuthorization();

            return endpoints;
        }
    }
}