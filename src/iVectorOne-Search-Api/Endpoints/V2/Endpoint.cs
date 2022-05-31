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
                    "/v2/property/search",
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
                        [FromQuery] string suppliers)
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
                            NaionalityID = nationalityid ?? string.Empty,
                            OpaqueRates = opaquerates.GetValueOrDefault(),
                            RoomRequests = roomRequestsFactory.Create(rooms),
                            SellingCountry = sellingcountry ?? string.Empty,
                            Suppliers = suppliers.Split(",").ToList()
                        };

                        return await EndpointBase.ExecuteRequest<Request, Response>(httpContext, mediator, request);
                    })
                .RequireAuthorization()
                .WithName("Property Search Query")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    "/property/search",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Request request)
                        => await EndpointBase.ExecuteRequest<Request, Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Search")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            return endpoints;
        }
    }
}