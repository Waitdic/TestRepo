using iVectorOne_Admin_Api.Features.V1.Tenants.Disable;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Disable
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantDisableV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/{tenantId}/disable", async (IMediator mediator, int tenantId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId });

                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}