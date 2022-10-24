using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Users.Create
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapUsersCreateV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/users/", async (
                IMediator mediator, 
                [FromBody] Request request) =>
            {
                var response = await mediator.Send(request);

                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}