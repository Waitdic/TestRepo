namespace iVectorOne_Admin_Api.Features.V1.Tenants.Create
{
    using Microsoft.AspNetCore.Mvc;

    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantCreateV1Endpoint(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints.MapPost("v1/tenants/", async (
                IMediator mediator,
                [FromBody] RequestDto requestDto,
                HttpRequest httpRequest) =>
            {

                var request = new Request
                {
                    UserKey = httpRequest.Headers["UserKey"].ToString(),
                    CompanyName = requestDto.CompanyName,
                    ContactEmail = requestDto.ContactEmail,
                    ContactName = requestDto.ContactName,
                    ContactTelephone = requestDto.ContactTelephone
                };

                var response = await mediator.Send(request);

                return response.Result;
            });

            return endpoints;
        }
    }
}