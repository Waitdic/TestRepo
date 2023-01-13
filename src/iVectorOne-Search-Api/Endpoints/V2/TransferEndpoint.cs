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
                        [FromQuery] int departureLocationId,
                        [FromQuery] int arrivalLocationId,
                        [FromQuery] DateTime departureDate,
                        [FromQuery] string? departureTime,
                        [FromQuery] bool? oneWay,
                        [FromQuery] DateTime? returnDate,
                        [FromQuery] string? returnTime,
                        [FromQuery] int? adults,
                        [FromQuery] int? children,
                        [FromQuery] int? infants,
                        [FromQuery] string? childAges,
                        [FromQuery] string? supplier,
                        [FromQuery] string? currencycode,
                        [FromQuery] string? emailLogsTo)
                    =>
                    {
                        var request = new Request
                        {
                            DepartureLocationID = departureLocationId,
                            ArrivalLocationID = arrivalLocationId,
                            DepartureDate = departureDate,
                            DepartureTime = departureTime ?? string.Empty,
                            OneWay = oneWay ?? true,
                            ReturnDate = returnDate,
                            ReturnTime = returnTime ?? string.Empty,
                            Adults = adults ?? 0,
                            Children = children ?? 0,
                            Infants = infants ?? 0,
                            ChildAges = SetChildAges(childAges ?? string.Empty),
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

        private static List<int> SetChildAges(string childAges)
        {
            return childAges.Split(',').Where(m => int.TryParse(m, out _)).Select(m => int.Parse(m)).ToList();
        }
    }
}