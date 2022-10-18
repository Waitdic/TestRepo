namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Unlink
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantUserUnlinkV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/{tenantId}/users/{userId}/unlink",
                async (
                    IMediator mediator,
                    int tenantId,
                    int userId
                    ) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId, UserId = userId });

                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}