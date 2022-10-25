using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Modify
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantModifyV1Endpoint(this IEndpointRouteBuilder endpoints)
        {

            _ = endpoints.MapPut("v1/tenants/{tenantId}", async (IMediator mediator, int tenantId, [FromBody] RequestDto requestDto) =>
            {
                var response = await mediator.Send(new Request
                {
                    TenantId = tenantId,
                    ContactEmail = requestDto.ContactEmail,
                    ContactName = requestDto.ContactName,
                    ContactTelephone = requestDto.ContactTelephone
                });

                return response.Result;
            });

            return endpoints;
        }
    }
}