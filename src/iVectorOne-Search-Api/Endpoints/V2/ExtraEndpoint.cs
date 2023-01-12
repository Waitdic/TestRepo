﻿using Intuitive.Helpers.Extensions;
using iVectorOne.SDK.V2.ExtraSearch;
using iVectorOne.Web.Infrastructure.V2;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace iVectorOne.Search.Api.Endpoints.V2
{
    public static class ExtraEndpoint
    {
        public static IEndpointRouteBuilder MapExtraEndpoints(this IEndpointRouteBuilder endpoints)
        {
            RegisterSearchEndpointsForDomain(endpoints, EndpointBase.ExtrasDomain);

            return endpoints;
        }

        private static void RegisterSearchEndpointsForDomain(IEndpointRouteBuilder endpoints, string domain)
        {
            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{domain}/search",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Request request)
                        => await EndpointBase.ExecuteRequest<Request, Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName($"{domain.ToProperCase()} Search")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);
        }
    }
}
