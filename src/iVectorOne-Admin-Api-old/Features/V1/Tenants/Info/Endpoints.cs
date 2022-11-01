namespace iVectorOne_Admin_Api.Features.V1.Tenants.Info
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantInfoV1Endpoint(this IEndpointRouteBuilder endpoints)
        {

            _ = endpoints.MapGet("v1/tenants/{tenantId}", async (IMediator mediator, int tenantId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId });

                return response.Result;
            });

            return endpoints;
        }
    }
}