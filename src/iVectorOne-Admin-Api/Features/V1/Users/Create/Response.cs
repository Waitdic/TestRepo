using iVectorOne_Admin_Api.Features.Shared;

namespace iVectorOne_Admin_Api.Features.V1.Users.Create
{
    public record Response : ResponseBase
    {
        public void Default(ResponseModel model)
        {
            Result = Results.Ok(model);
        }

        public void BadRequest()
        {
            Result = Results.BadRequest();
        }
    }
}