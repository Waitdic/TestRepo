using iVectorOne_Admin_Api.Config.Models;
using iVectorOne_Admin_Api.Features.Shared;
using System.Text.Json.Serialization;

namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    public record Response : ResponseBase
    {
        public void Default(ResponseModel user)
        {
            Result = Results.Ok(user);
        }

        public void NotFound()
        {
            Result = Results.NotFound();
        }
    }
}