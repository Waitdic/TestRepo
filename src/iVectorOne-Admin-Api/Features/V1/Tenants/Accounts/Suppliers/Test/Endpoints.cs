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
                    var response = await mediator.Send(new Request
                    {
                        AccountID = accountId,
                        SupplierID = supplierId,
                    });

                    return response.Result;

                }).RequireAuthorization();

            return endpoints;
        }
    }
}