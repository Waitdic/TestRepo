using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantAccountSupplierTestV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}/test",
                async (
                    IMediator mediator,
                    HttpContext httpContext,
                    int accountId,
                    int supplierId) =>
                {
                    var response = await mediator.Send(new Post.Request
                    {
                        AccountID = accountId,
                        SupplierID = supplierId,
                    });

                    return response.Result;

                }).RequireAuthorization();

            _ = endpoints.MapGet("v1/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}/test", async (
                IMediator mediator,
                int accountId, int supplierId,
                [FromQuery] string q) =>
                {
                    var request = new Get.Request
                    {
                        AccountID = accountId,
                        RequestKey = q
                    };

                    var response = await mediator.Send(request);

                    return response.Result;
                }).RequireAuthorization();

            return endpoints;
        }
    }
}