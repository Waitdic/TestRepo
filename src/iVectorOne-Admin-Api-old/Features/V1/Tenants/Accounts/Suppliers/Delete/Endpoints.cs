namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Delete
{
    using Microsoft.AspNetCore.Mvc;

    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantAccountSupplierDeleteV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapDelete("v1/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}",
                async (IMediator mediator, HttpContext httpContext, [FromHeader(Name = "TenantKey")] Guid tenantKey,
                    int tenantId, int accountId, int supplierId) =>
                {
                    var response = await mediator.Send(new Request
                        {TenantId = tenantId, AccountId = accountId, SupplierId = supplierId});

                    return response.Result;

                }).RequireAuthorization();

            return endpoints;
        }
    }
}