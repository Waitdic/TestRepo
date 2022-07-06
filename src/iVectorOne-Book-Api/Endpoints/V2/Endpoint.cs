namespace iVectorOne.Book.Api.Endpoints.V2
{
    using FluentValidation;
    using iVectorOne.Web.Infrastructure;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Prebook = ThirdParty.SDK.V2.PropertyPrebook;
    using Book = ThirdParty.SDK.V2.PropertyBook;
    using Precancel = ThirdParty.SDK.V2.PropertyPrecancel;
    using Cancel = ThirdParty.SDK.V2.PropertyCancel;

    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{EndpointBase.Domain}/prebook",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Prebook.Request request)
                        => await EndpointBase.ExecuteRequest<Prebook.Request, Prebook.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Prebook")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{EndpointBase.Domain}/book",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Book.Request request)
                        => await EndpointBase.ExecuteRequest<Book.Request, Book.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Book")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{EndpointBase.Domain}/cancelfee",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Precancel.Request request)
                        => await EndpointBase.ExecuteRequest<Precancel.Request, Precancel.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Precancel")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{EndpointBase.Domain}/cancel",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Cancel.Request request)
                        => await EndpointBase.ExecuteRequest<Cancel.Request, Cancel.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Cancel")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            return endpoints;
        }
    }
}