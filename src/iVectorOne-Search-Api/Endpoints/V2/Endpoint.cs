namespace iVectorOne.Search.Api.Endpoints.V2
{
    using FluentValidation;
    using iVectorOne.Web.Infrastructure;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using ThirdParty.Factories;
    using ThirdParty.SDK.V2.PropertySearch;

    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints
                .MapGet(
                    $"/{EndpointBase.Version}/{EndpointBase.Domain}/search",
                    async (
                        HttpContext httpContext,
                        [FromServices] IMediator mediator,
                        [FromServices] IRoomRequestsFactory roomRequestsFactory,
                        [FromQuery] DateTime arrivalDate,
                        [FromQuery] int duration,
                        [FromQuery] string properties,
                        [FromQuery] string rooms,
                        [FromQuery] string? nationalityid,
                        [FromQuery] bool? opaquerates,
                        [FromQuery] string? sellingcountry,
                        [FromQuery] string? currencycode,
                        [FromQuery] bool? log, // todo - move to config
                        [FromQuery] string? suppliers,
                        [FromQuery] string? emailLogsTo)
                    =>
                    {
                        var request = new Request
                        {
                            ArrivalDate = arrivalDate,
                            Duration = duration,
                            Properties = properties.Split(',')
                                .Where(m => int.TryParse(m, out _))
                                .Select(m => int.Parse(m))
                                .ToList(),
                            CurrencyCode = currencycode ?? string.Empty,
                            NationalityID = nationalityid ?? string.Empty,
                            OpaqueRates = opaquerates.GetValueOrDefault(),
                            RoomRequests = roomRequestsFactory.Create(rooms),
                            SellingCountry = sellingcountry ?? string.Empty,
                            Suppliers = suppliers?.Split(",").ToList() ?? new(),
                            EmailLogsToAddress = emailLogsTo ?? string.Empty,
                        };

                        return await EndpointBase.ExecuteRequest<Request, Response>(httpContext, mediator, request);
                    })
                .RequireAuthorization()
                .WithName("Property Search Query")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{EndpointBase.Domain}/search",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Request request)
                        => await EndpointBase.ExecuteRequest<Request, Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Search")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints.MapGet("/healthcheck", () => "Hello World!");

            return endpoints;
        }
    }
}