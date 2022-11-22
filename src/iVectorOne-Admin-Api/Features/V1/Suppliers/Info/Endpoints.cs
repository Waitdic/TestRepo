namespace iVectorOne_Admin_Api.Features.V1.Suppliers.Info
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapSupplierInfoV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/suppliers/{supplierid}", async (IMediator mediator, HttpContext httpContext, int supplierid) =>
            {
                var response = await mediator.Send(new Request { SupplierID = supplierid});
                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}