using iVectorOne_Admin_Api.Config.Requests;
using iVectorOne_Admin_Api.Config.Responses;

namespace iVectorOne_Admin_Api.Features.V2.Tenants.Accounts.Suppliers.Info
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantAccountSupplierInfoV2Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v2/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}", async (IMediator mediator, HttpContext httpContext, int tenantid, int accountid, int supplierid) =>
            {
                SupplierResponse response = null!;

                try
                {
                    var request = new SupplierRequest(tenantid) { AccountId = accountid, SupplierId = supplierid };
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