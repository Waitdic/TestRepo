using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapSearchTestV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/{tenantid}/accounts/{accountid}/search", async (
                IMediator mediator,
                [FromBody] SearchRequest requestBody,
                int tenantId, int accountId) =>
            {
                var request = new Request
                {
                    AccountID = accountId,
                    SearchRequest = requestBody
                };

                var response = await mediator.Send(request);

                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}