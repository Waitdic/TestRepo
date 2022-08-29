namespace iVectorOne_Admin_Api.Features.Shared
{
    using iVectorOne_Admin_Api.Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public abstract record ResponseBase
    {
        public ResponseBase()
        {
            var activity = Activity.Current;

            var problemDetails = new ProblemDetails
            {
                Title = "An unexpected error occurred processing your request.",
            };

            problemDetails.Extensions.Add(new KeyValuePair<string, object?>("Request-Id", activity?.GetTraceId()));

            Result =  Results.Problem(problemDetails);
        }

        public IResult Result { get; set; } 
    }
}