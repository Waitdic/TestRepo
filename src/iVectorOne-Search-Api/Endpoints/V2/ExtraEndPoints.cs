using Intuitive.Helpers.Extensions;
using iVectorOne.SDK.V2.ExtraSearch;
using iVectorOne.Web.Infrastructure.V2;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace iVectorOne.Search.Api.Endpoints.V2
{
    public static class ExtraEndPoints
    {
        public static IEndpointRouteBuilder MapExtraEndpoints(this IEndpointRouteBuilder endpoints)
        {
            RegisterSearchEndpointsForDomain(endpoints, EndpointBase.ExtrasDomain);

            _ = endpoints.MapGet("/healthcheck", () => "Hello World!").AllowAnonymous();

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

        private static List<int> SetChildAges(string childAges)
        {
            return childAges.Split(',').Where(m => int.TryParse(m, out _)).Select(m => int.Parse(m)).ToList();
        }

    }
}
