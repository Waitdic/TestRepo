namespace iVectorOne.Search.Api.Endpoints.V2
{
    using FluentValidation;
    using Intuitive.Helpers.Extensions;
    using iVectorOne.Factories;
    using iVectorOne.Models;
    using iVectorOne.SDK.V2.TransferSearch;
    using iVectorOne.Web.Infrastructure.V2;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    public static class TransferEndpoint
    {
        public static IEndpointRouteBuilder MapTransferEndpoints(this IEndpointRouteBuilder endpoints)
        {
            RegisterSearchEndpointsForDomain(endpoints, EndpointBase.TransferDomain);

            _ = endpoints.MapGet("/healthcheck", () => "Hello World!").AllowAnonymous();

            return endpoints;
        }

        private static void RegisterSearchEndpointsForDomain(IEndpointRouteBuilder endpoints, string domain)
        {
            _ = endpoints
                .MapGet(
                    $"/{EndpointBase.Version}/{domain}/search",
                    async (
                        HttpContext httpContext,
                        [FromServices] IMediator mediator,
                        [FromQuery] DateTime departureDate,
                        [FromQuery] int? departureLocationId,
                        [FromQuery] int? arrivalLocationId,
                        [FromQuery] int? adults,
                        [FromQuery] int? children,
                        [FromQuery] int? infants,
                        [FromQuery] string? supplier,
                        [FromQuery] string? currencycode,
                        [FromQuery] string? emailLogsTo)
                    =>
                    {
                        var request = new Request
                        {
                            DepartureDate = departureDate,
                            DepartureLocationID = departureLocationId ?? 0,
                            ArrivalLocationID = arrivalLocationId ?? 0,
                            Adults = adults ?? 0,
                            Children = children ?? 0,
                            Infants = infants ?? 0,
                            CurrencyCode = currencycode ?? string.Empty,
                            //SellingCountry = sellingcountry ?? string.Empty,
                            Supplier = supplier ?? string.Empty,
                            EmailLogsToAddress = emailLogsTo ?? string.Empty,
                        };

                        return await EndpointBase.ExecuteRequest<Request, Response>(httpContext, mediator, request);
                    })
                .RequireAuthorization()
                .WithName($"{domain.ToProperCase()} Search Query")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

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