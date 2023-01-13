using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Properties.Search
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapPropertiesSearchV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/tenants/{tenantid}/accounts/{accountid}/properties", async (
                IMediator mediator,
                int tenantId,
                int accountId,
                [FromQuery] string? query) =>
            {
                var response = await mediator.Send(new Request
                {
                    TenantId = tenantId,
                    AccountId = accountId,
                    Query = query
                });

                return response.Result;
            });

            return endpoints;
        }
    }
}
