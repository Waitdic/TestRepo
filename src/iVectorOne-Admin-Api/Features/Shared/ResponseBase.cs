using iVectorOne_Admin_Api.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace iVectorOne_Admin_Api.Features.Shared
{
    public record ResponseBase
    {
        public ResponseBase()
        {
            Result = Results.Problem(BuildProblemDetails(""));
        }

        public IResult Result { get; set; }

        public void NotFound()
        {
            Result = Results.NotFound();
        }

        public void NotFound(string detail) => Result = Results.NotFound(BuildProblemDetails(detail));

        public void Ok(IResponseModel model)
        {
            Result = Results.Ok(model);
        }

        public void BadRequest(string detail) => Result = Results.BadRequest(BuildProblemDetails(detail));

        #region Private methods

        private static ProblemDetails BuildProblemDetails(string detail)
        {
            var activity = Activity.Current;

            return new ProblemDetails
            {
                Title = "An error occurred while processing your request.",
                Instance = activity?.GetTraceId(),
                Detail = detail
            };
        }

        #endregion
    }
}