namespace iVectorOne_Admin_Api.Features.V1.Users.Create
{
    public record ResponseModel : ResponseModelBase
    {
        public int UserId { get; set; }
    }
}
