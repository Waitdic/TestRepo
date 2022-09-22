namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Create
{
    using Microsoft.AspNetCore.Mvc;

    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantAccountCreateV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/{tenantid}/accounts", async (IMediator mediator,
                HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantId,
                [FromBody] AccountDto account) =>
            {
                var response = await mediator.Send(new Request {TenantId = tenantId, Account = account });

                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}