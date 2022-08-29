namespace iVectorOne_Admin_Api.Features.V1.Users.Info
{
    using iVectorOne_Admin_Api.Features.Shared;

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