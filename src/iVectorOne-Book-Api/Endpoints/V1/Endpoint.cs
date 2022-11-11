﻿namespace iVectorOne.Book.Api.Endpoints.V1
{
    using FluentValidation;
    using iVectorOne.Web.Infrastructure.V1;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Book = SDK.V2.PropertyBook;
    using Cancel = SDK.V2.PropertyCancel;
    using Prebook = SDK.V2.PropertyPrebook;
    using Precancel = SDK.V2.PropertyPrecancel;

    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapEndpointsV1(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Domain}/prebook",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Prebook.Request request)
                        => await EndpointBase.ExecuteRequest<Prebook.Request, Prebook.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Prebook V1")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Domain}/book",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Book.Request request)
                        => await EndpointBase.ExecuteRequest<Book.Request, Book.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Book V1")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Domain}/cancelfee",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Precancel.Request request)
                        => await EndpointBase.ExecuteRequest<Precancel.Request, Precancel.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Cancelfee")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            _ = endpoints
                .MapPost(
                    $"/{EndpointBase.Domain}/cancel",
                    async (HttpContext httpContext, [FromServices] IMediator mediator, [FromBody] Cancel.Request request)
                        => await EndpointBase.ExecuteRequest<Cancel.Request, Cancel.Response>(httpContext, mediator, request))
                .RequireAuthorization()
                .WithName("Property Cancel V1")
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            return endpoints;
        }
    }
}