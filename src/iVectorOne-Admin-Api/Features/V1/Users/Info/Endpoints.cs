namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapUsersListV1Endpoint(this IEndpointRouteBuilder endpoints)
        {

            _ = endpoints.MapGet("v1/users/{key}", async (IMediator mediator, string key) =>
            {
                var response = await mediator.Send(new Request { Key = key });

                return response.Result;
            });

            return endpoints;
        }
    }
}