namespace iVectorOne.Book.Api.Endpoints.V2
{
    using FluentValidation;
    using iVectorOne.Web.Infrastructure.V2;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Book = SDK.V2.TransferBook;
    using Cancel = SDK.V2.TransferCancel;
    using Prebook = SDK.V2.TransferPrebook;
    using Precancel = SDK.V2.TransferPrecancel;

    public static class TransferEndpoint
    {
        public static IEndpointRouteBuilder MapTransferEndpoints(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{EndpointBase.TransferDomain}/prebook",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Prebook.Request request)
                        => await EndpointBase.ExecuteRequest<Prebook.Request, Prebook.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Transfer Prebook")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{EndpointBase.TransferDomain}/book",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Book.Request request)
                        => await EndpointBase.ExecuteRequest<Book.Request, Book.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Transfer Book")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{EndpointBase.TransferDomain}/precancel",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Precancel.Request request)
                        => await EndpointBase.ExecuteRequest<Precancel.Request, Precancel.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Transfer Precancel")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Version}/{EndpointBase.TransferDomain}/cancel",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Cancel.Request request)
                        => await EndpointBase.ExecuteRequest<Cancel.Request, Cancel.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Transfer Cancel")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            return endpoints;
        }
    }
}