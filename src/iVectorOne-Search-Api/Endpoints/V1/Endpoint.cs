namespace iVectorOne.Search.Api.Endpoints.V1
{
	using FluentValidation;
	using iVectorOne.Web.Infrastructure;
	using MediatR;
	using Microsoft.AspNetCore.Mvc;
	using iVectorOne.Factories;
	using iVectorOne.SDK.V2.PropertySearch;
	using Intuitive.Helpers.Extensions;
	using iVectorOne.Models;

	public static class Endpoint
	{
		public static IEndpointRouteBuilder MapEndpointsV1(this IEndpointRouteBuilder endpoints)
		{
			_ = endpoints
				.MapGet(
					"/property/search",
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
						[FromQuery] string? emailLogsTo,
						[FromQuery] string? dedupeMethod)
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
							DedupeMethod = dedupeMethod.ToSafeEnum<DedupeMethod>() ?? DedupeMethod.cheapestleadin
						};

						return await EndpointBase.ExecuteRequest<Request, Response>(httpContext, mediator, request);
					})
				.RequireAuthorization()
				.WithName("Property Search Query V1")
				.ProducesValidationProblem(StatusCodes.Status400BadRequest)
				.Produces(StatusCodes.Status200OK);

			_ = endpoints
				.MapPost(
					"/property/search",
					async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Request request)
						=> await EndpointBase.ExecuteRequest<Request, Response>(httpContext, mediator, request))
				.RequireAuthorization()
				.WithName("Property Search V1")
				.ProducesValidationProblem(StatusCodes.Status400BadRequest)
				.Produces(StatusCodes.Status200OK);

			return endpoints;
		}
	}
}