namespace iVectorOne.Content.Api.Endpoints.V1
{
	using FluentValidation;
	using MediatR;
	using iVectorOne.Web.Infrastructure;
	using Microsoft.AspNetCore.Mvc;
	using List = SDK.V2.PropertyList;
	using Content = SDK.V2.PropertyContent;
	using Intuitive.Helpers.Extensions;

	public static class Endpoint
	{
		public static IEndpointRouteBuilder MapEndpointsV1(this IEndpointRouteBuilder endpoints)
		{
			RegisterContentEndpointsForDomain(endpoints, "/content/property/list"); // v1 call
			RegisterContentEndpointsForDomain(endpoints, "/content/property"); // v1 call

			return endpoints;
		}

		private static void RegisterContentEndpointsForDomain(IEndpointRouteBuilder endpoints, string domain)
		{
			_ = endpoints
				.MapGet(
					domain,
					async (
						HttpContext httpContext,
						[FromServices] IMediator mediator,
						[FromQuery] string? propertyids,
						[FromQuery] DateTime? lastModified,
						[FromQuery] string? suppliers)
						=>
					{
						var propertyIdList = (propertyids ?? string.Empty).Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();

						if (propertyIdList.Any())
						{
							var request = new Content.Request
							{
								PropertyIDs = propertyIdList,
							};

							return await EndpointBase.ExecuteRequest<Content.Request, Content.Response>(httpContext, mediator, request);
						}
						else
						{
							var request = new List.Request
							{
								LastModified = lastModified,
								Suppliers = suppliers ?? string.Empty,
							};

							return await EndpointBase.ExecuteRequest<List.Request, List.Response>(httpContext, mediator, request);
						}
					})
				.RequireAuthorization()
				.WithName($"{domain.ToProperCase()} Content V1")
				.ProducesValidationProblem(StatusCodes.Status400BadRequest)
				.ProducesValidationProblem(StatusCodes.Status401Unauthorized)
				.Produces(StatusCodes.Status200OK);

		}
	}
}