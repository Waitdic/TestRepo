using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Utilities.LogViewer
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapLogViewerV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapGet("v1/tenants/{tenantid}/accounts/{accountid}/logs", async (
                IMediator mediator,
                int tenantId, 
                int accountId,
                [FromQuery] DateTime? startDate,
                [FromQuery] DateTime? endDate,
                [FromQuery] string enviroment,
                [FromQuery] string type,
                [FromQuery] string status) =>
            {
                 var request = new Request
                {
                    AccountID = accountId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Environment = enviroment,
                    Type = type,
                    Status = status
                };

                var response = await mediator.Send(request);

                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}
