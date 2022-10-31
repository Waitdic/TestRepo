using iVectorOne_Admin_Api.Features.V1.Tenants.Delete;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Delete
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantDeleteV1Endpoint(this IEndpointRouteBuilder endpoints)
        {

            _ = endpoints.MapDelete("v1/tenants/{tenantId}", async (IMediator mediator, int tenantId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId });

                return response.Result;
            });

            return endpoints;
        }
    }
}