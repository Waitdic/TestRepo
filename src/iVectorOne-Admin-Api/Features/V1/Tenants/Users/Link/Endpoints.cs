using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Users.Link
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantUserLinkV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/{tenantId}/users/{userId}/link",
                async (
                    IMediator mediator,
                    int tenantId,
                    int userId,
                    [FromBody] RequestDto requestDto
                    ) =>
            {
                var response = await mediator.Send(new Request { TenantId = tenantId, UserId = userId, Relationship = requestDto.RelationShip });

                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}