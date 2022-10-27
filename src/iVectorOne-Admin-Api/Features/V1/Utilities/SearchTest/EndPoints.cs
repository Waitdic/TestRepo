using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.SearchTest
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapSearchTestV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/{tenantid}/accounts/{accountid}/search", async (
                IMediator mediator,
                [FromBody] Post.SearchRequest requestBody,
                int tenantId, int accountId) =>
            {
                var request = new Post.Request
                {
                    AccountID = accountId,
                    SearchRequest = requestBody
                };

                var response = await mediator.Send(request);

                return response.Result;
            }).RequireAuthorization();

            _ = endpoints.MapGet("v1/tenants/{tenantid}/accounts/{accountid}/search", async (
                IMediator mediator,
                int tenantId, int accountId,
                [FromQuery] string q) =>
                {
                    var request = new Get.Request
                    {
                        AccountID = accountId,
                        RequestKey = q
                        //SearchRequest = requestBody
                    };

                    var response = await mediator.Send(request);

                    return response.Result;
                }).RequireAuthorization();

            return endpoints;
        }
    }
}