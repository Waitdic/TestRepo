namespace iVectorOne_Admin_Api.Features.V1.Users.Create
{
    using Microsoft.AspNetCore.Mvc;

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
            });

            return endpoints;
        }
    }
}