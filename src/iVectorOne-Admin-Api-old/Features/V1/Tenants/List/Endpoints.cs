namespace iVectorOne_Admin_Api.Features.V1.Tenants.List
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapUsersTenantsListV1Endpoint(this IEndpointRouteBuilder endpoints)
        {

            _ = endpoints.MapGet("v1/tenants", async (IMediator mediator) =>
            {
                var response = await mediator.Send(new Request ());

                return response.Result;
            });

            return endpoints;
        }
    }
}