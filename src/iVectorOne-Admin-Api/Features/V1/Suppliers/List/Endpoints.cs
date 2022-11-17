namespace iVectorOne_Admin_Api.Features.V1.Suppliers.List
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapSupplierListV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/suppliers", async (IMediator mediator) =>
            {
                var response = await mediator.Send(new Request());

                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}