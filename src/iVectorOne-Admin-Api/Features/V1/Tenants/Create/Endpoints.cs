using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Create
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantCreateV1(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/", async (
                IMediator mediator,
                [FromBody] RequestBody requestBody,
                HttpRequest httpRequest) =>
            {
                var request = new Request
                {
                    UserKey = httpRequest.Headers["UserKey"].ToString(),
                    CompanyName = requestBody.CompanyName,
                    ContactEmail = requestBody.ContactEmail,
                    ContactName = requestBody.ContactName,
                    ContactTelephone = requestBody.ContactTelephone
                };

                var response = await mediator.Send(request);

                return response.Result;
            }).RequireAuthorization();

            return endpoints;
        }
    }
}