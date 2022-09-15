﻿namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Info
{
    using Microsoft.AspNetCore.Mvc;

    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantAccountInfoV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/tenants/{tenantid}/accounts/{accountid}", async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantId, int accountId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId, AccountId = accountId });

                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}