namespace iVectorOne.Content.Api.Endpoints.V2
{
    using FluentValidation;
    using iVectorOne.Web.Infrastructure.V2;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Content = SDK.V2.PropertyContent;
    using List = SDK.V2.PropertyList;

    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints
                .MapGet(
                    $"/{EndpointBase.Version}/{EndpointBase.Domain}",
                    async (
                        HttpContext httpContext,
                        [FromServices] IMediator mediator,
                        [FromQuery] string? propertyids,
                        [FromQuery] DateTime? lastModified,
                        [FromQuery] string? suppliers,
                        [FromQuery] bool? includeRoomTypes)
                        =>
                        {
                            var propertyIdList = (propertyids ?? string.Empty).Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();

                            if (propertyIdList.Any())
                            {
                                var request = new Content.Request
                                {
                                    PropertyIDs = propertyIdList,
                                    IncludeRoomTypes = includeRoomTypes ?? false,
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
                .WithName("Property Content")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .ProducesValidationProblem(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status200OK);

            // todo - integrate with health checks
            _ = endpoints.MapGet("/healthcheck", () => "Hello World!").AllowAnonymous();

            return endpoints;
        }
    }
}