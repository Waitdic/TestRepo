﻿using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Subscriptions.Delete
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantSubscriptionDeleteV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapDelete("v1/tenants/{tenantid}/subscriptions/{subscriptionid}", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantId, int subscriptionId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId, SubscriptionId = subscriptionId });

                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}