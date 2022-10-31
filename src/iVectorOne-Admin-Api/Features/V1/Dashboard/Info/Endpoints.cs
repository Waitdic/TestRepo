using iVectorOne_Admin_Api.Data;

namespace iVectorOne_Admin_Api.Features.V1.Dashboard.Info
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapDashboardInfoV1Endpoint(this IEndpointRouteBuilder endpoints)
        {

            _ = endpoints.MapGet("v1/tenants/{tenantid}/accounts/{accountid}/dashboard", async (IMediator mediator, int tenantId, int accountId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId, AccountId = accountId });

                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}