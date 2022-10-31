using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using iVectorOne_Admin_Api.Security;

namespace iVectorOne_Admin_Api.Features.V2.Suppliers.Info
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapSupplierInfoV2Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v2/suppliers/{supplierid}", async (IMediator mediator, HttpContext httpContext, int supplierid) =>
            {
                if (httpContext.User.Identity is not TenantIdentity identity)
                {
                    return Results.Challenge();
                }

                SupplierAttributeResponse response = null!;

                try
                {
                    var request = new SupplierAttributeRequest() { SupplierID = supplierid };
                    response = await mediator.Send(request);
                }
                catch (Exception e)
                {
                    return Results.Problem(e.ToString());
                }

                return Results.Ok(response);
            }).RequireAuthorization();

            return endpoints;
        }
    }
}