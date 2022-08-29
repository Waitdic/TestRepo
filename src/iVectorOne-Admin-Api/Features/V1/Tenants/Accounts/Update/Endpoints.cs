namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Update
{
    using Microsoft.AspNetCore.Mvc;

    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantAccountUpdateV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPut("v1/tenants/{tenantid}/accounts/{accountid}", async (IMediator mediator,
                HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey, int tenantId,
                int accountId,
                [FromBody] AccountDto account) =>
            {
                var response = await mediator.Send(new Request {TenantId = tenantId, AccountId = accountId, Account = account});

                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}