using iVectorOne_Admin_Api.Config.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.List
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantSubscriptionsListV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/tenants/{tenantid}/subscriptions", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId });

                return response.Result;

                //if (httpContext.User.Identity is not TenantIdentity identity)
                //{
                //    return Results.Challenge();
                //}

                //TenantResponse response = default;

                //try
                //{
                //    var request = new TenantRequest(tenantid);
                //    response = await mediator.Send(request);
                //}
                //catch (Exception e)
                //{
                //    return Results.Problem(e.ToString());
                //}

                //return Results.Ok(response);
            }).RequireAuthorization();

            return endpoints;
        }
    }
}