namespace iVectorOne_Admin_Api.Features.V1.Tenants.Accounts.Suppliers.Info
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantAccountSupplierInfoV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/tenants/{tenantid}/accounts/{accountid}/suppliers/{supplierid}", async (IMediator mediator, int tenantid, int accountid, int supplierid) =>
            {
                var response = await mediator.Send(new Request { TenantID = tenantid, AccountID = accountid, SupplierID = supplierid });
                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}