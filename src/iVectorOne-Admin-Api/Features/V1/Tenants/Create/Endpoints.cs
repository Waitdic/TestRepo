﻿using iVectorOne_Admin_Api.Features.V1.Tenants.Create;
using Microsoft.AspNetCore.Mvc;

namespace iVectorOne_Admin_Api.Features.V1.Tenants.Create
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTenantCreateV1Endpoint(this IEndpointRouteBuilder endpoints)
        {

            _ = endpoints.MapPost("v1/tenants/", async (
                IMediator mediator, 
                [FromBody] Request request) =>
            {
                var response = await mediator.Send(request);

                return response.Result;
            });

            return endpoints;
        }
    }
}