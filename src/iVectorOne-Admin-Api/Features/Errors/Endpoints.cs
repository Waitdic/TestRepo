using iVectorOne_Admin_Api.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace iVectorOne_Admin_Api.Features.Errors
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapErrorRoutes(this IEndpointRouteBuilder endpoints)
        {

            _ = endpoints.Map("/error", (HttpContext httpContext) =>
            {
                var activity = Activity.Current;

                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred processing your request. Please try again. If the problem persists log a call with our help desk quoting the Instance for this response.",
                    Instance = activity?.GetTraceId()
                };

                return Results.Problem(problemDetails);
            })
            .AllowAnonymous()
            .ExcludeFromDescription();

            return endpoints;
        }
    }
}