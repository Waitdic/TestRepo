//using iVectorOne_Admin_Api.Config.Requests;
//using iVectorOne_Admin_Api.Config.Responses;
//using iVectorOne_Admin_Api.Security;

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

                //if (httpContext.User.Identity is not TenantIdentity identity)
                //{
                //    return Results.Challenge();
                //}

                //Response response = null!;

                //try
                //{
                //    var request = new Request() { SupplierID = supplierid };
                //    response = await mediator.Send(request);
                //}
                //catch (Exception e)
                //{
                //    return Results.Problem(e.ToString());
                //}

                //return Results.Ok(response);
            }).RequireAuthorization();

            return endpoints;
        }
    }
}