namespace iVectorOne_Admin_Api.Features.V1.Tenants.Enable
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantEnableV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/{tenantId}/enable", async (IMediator mediator, int tenantId) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId });
                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}