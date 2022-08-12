using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Users.List
{
    public record Response : ResponseBase
    {
        public void Default(ResponseModel model)
        {
            Result = Results.Ok(model);
        }

        public void NotFound()
        {
            Result = Results.NotFound();
        }
    }
}