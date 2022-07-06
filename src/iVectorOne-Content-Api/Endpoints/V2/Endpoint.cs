namespace iVectorOne.Content.Api.Endpoints.V2
{
    using FluentValidation;
    using MediatR;
    using iVectorOne.Web.Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using List = ThirdParty.SDK.V2.PropertyList;
    using Content = ThirdParty.SDK.V2.PropertyContent;

    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints
                .MapGet(
                    $"/{EndpointBase.Version}/{EndpointBase.Domain}/list",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromQuery] DateTime? lastModified, [FromQuery] string? suppliers)
                        =>
                        {
                            var request = new List.Request
                            {
                                LastModified = lastModified,
                                Suppliers = suppliers ?? string.Empty,
                            };

                            return await EndpointBase.ExecuteRequest<List.Request, List.Response>(httpContext, mediator, request);
                        })
                .RequireAuthorization()
                .WithName("Property List")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .ProducesValidationProblem(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapGet(
                    $"/{EndpointBase.Version}/{EndpointBase.Domain}",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromQuery] string propertyids)
                        =>
                        {
                            var request = new Content.Request
                            {
                                PropertyIDs = propertyids,
                            };

                            return await EndpointBase.ExecuteRequest<Content.Request, Content.Response>(httpContext, mediator, request);
                        })
                .RequireAuthorization()
                .WithName("Property Content")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            return endpoints;
        }
    }
}