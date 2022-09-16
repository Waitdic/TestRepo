namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Test
{
    using Microsoft.AspNetCore.Mvc;

    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantAccountSupplierTestV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}/test",
                async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey,
                    int accountId, int supplierId) =>
                {
                    var response = await mediator.Send(new Request
                        { AccountID = accountId, SupplierID = supplierId });

                    return response.Result;

                }).RequireAuthorization();

            return endpoints;
        }
    }
}