namespace iVectorOne.Web.Infrastructure.V1
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using iVectorOne.Web.Infrastructure.Security;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using iVectorOne.SDK.V2;
    using System;

    public static class EndpointBase
    {
        public const string Domain = "property";

        public static async Task<IResult> ExecuteRequest<TRequest, TResponse>(HttpContext httpContext, IMediator mediator, TRequest request)
            where TRequest : RequestBase, IRequest<TResponse>
            where TResponse : ResponseBase, new()
        {
            if (httpContext.User.Identity is not AuthenticationIdentity identity)
            {
                return Results.Challenge();
            }
            request.Account = identity.Account;
            var response = new TResponse();

            try
            {
                response = await mediator.Send(request);

                if (response is null)
                {
                    return Results.UnprocessableEntity();
                }
                else if (response.Warnings?.Any() ?? false)
                {
                    return Results.BadRequest(new ErrorMessage(
                        string.Join(Environment.NewLine, response.Warnings)));
                }
            }
            catch (ValidationException ex)
            {
                return Results.BadRequest(new ErrorMessage(
                    string.Join(Environment.NewLine, ex.Errors.Select(e => e.ErrorMessage))));
            }

            return Results.Ok(response);
        }

        private class ErrorMessage
        {
            public ErrorMessage(string message)
            {
                Message = message;
            }

            public string Message { get; set; } = string.Empty;
        }
    }
}