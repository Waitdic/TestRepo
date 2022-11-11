using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewerDetail
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapLogViewerDetailV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/tenants/{tenantid}/accounts/{accountid}/logs/{key}", async (
                IMediator mediator,
                int tenantId,
                int accountId,
                int key) =>
            {
                var request = new Request
                {
                    Key = key
                };

                var response = await mediator.Send(request);

                return response.Result;

            }).RequireAuthorization();

            return endpoints;
        }
    }
}
