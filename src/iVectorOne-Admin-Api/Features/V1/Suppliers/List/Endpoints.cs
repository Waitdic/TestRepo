using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;
using iVectorOne_Admin_Api.Security;

namespace iVectorOne_Admin_Api.Features.V1.Suppliers.List
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapSupplierListV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/suppliers", async (IMediator mediator, HttpContext httpContext) =>
            {
                if (httpContext.User.Identity is not TenantIdentity identity)
                {
                    return Results.Challenge();
                }

                SupplierListResponse response = null!;

                try
                {
                    var request = new SupplierListRequest();
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