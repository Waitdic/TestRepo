namespace iVectorOne_Admin_Api.Features.V1.Users.List
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapUsersListV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/users", async (IMediator mediator) =>
            {
                var response = await mediator.Send(new Request());

                return response.Result;
            }); //.RequireAuthorization();

            return endpoints;
        }
    }
}